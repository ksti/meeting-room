using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Repositories;
using MeetingRoom.Api.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetingRoom.Api.Enums;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ApiSettings _apiSettings;

        public AuthService(
            IUserRepository userRepository, 
            ITokenRepository tokenRepository,
            JwtSettings jwtSettings, 
            ApiSettings apiSettings, 
            IDeviceRepository deviceRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _jwtSettings = jwtSettings;
            _apiSettings = apiSettings;
            _deviceRepository = deviceRepository;
        }

        public async Task<AuthResult> LoginAsync(string email, string password, DeviceInfo deviceInfo)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new BusinessException("Invalid email or password");
            }

            if (user.Status == UserStatus.Disabled.GetDisplayName())
            {
                throw new BusinessException("User account is disabled");
            }

            var tokenEntity = await GetAssignedToken(user, deviceInfo);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = user.MapToModel()
            };
        }

        public async Task<AuthResult> LoginByUsernameAsync(string username, string password, DeviceInfo deviceInfo)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new BusinessException("Invalid username or password");
            }

            if (user.Status == "disabled")
            {
                throw new BusinessException("User account is disabled");
            }

            var tokenEntity = await GetAssignedToken(user, deviceInfo);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = user.MapToModel()
            };
        }

        public async Task<AuthResult> RegisterAsync(UserCreateRequest request, DeviceInfo deviceInfo)
        {
            var existedUser = await _userRepository.GetByEmailAsync(request.Email!);
            if (existedUser != null)
            {
                throw new BusinessException("User already existed");
            }

            var newUser = new UserEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            var tokenEntity = await GenerateNewToken(newUser);
            var device = GenerateDevice(deviceInfo);
            device.Token = tokenEntity;
            newUser.Tokens.Add(tokenEntity);
            newUser.Devices.Add(device);
            await _userRepository.CreateAsync(newUser);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = newUser.MapToModel()
            };
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<AuthResult> RefreshTokenAsync(string userId, string refreshToken, DeviceInfo deviceInfo)
        {
            var token = await _tokenRepository.GetByRefreshTokenAsync(refreshToken);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || token == null || token.RefreshTokenExpiresAt < DateTime.UtcNow)
            {
                throw new BusinessException("Invalid refresh token");
            }

            var tokenEntity = await GetAssignedToken(user, deviceInfo, true);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = user.MapToModel()
            };
        }

        public async Task<bool> LogoutAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            user.Tokens = new List<TokenEntity>();
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> CheckAuthStatusAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(UserEntity userEntity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userEntity.Username),
                    new Claim(ClaimTypes.Email, userEntity.Email),
                    new Claim(ClaimTypes.Role, userEntity.Role),
                    new Claim("id", userEntity.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private DeviceEntity GenerateDevice(DeviceInfo deviceInfo)
        {
            return new DeviceEntity
            {
                DeviceIdentifier = deviceInfo.DeviceIdentifier,
                DeviceName = deviceInfo.DeviceName,
                AppVersion = deviceInfo.AppVersion,
                Browser = deviceInfo.Browser,
                BrowserVersion = deviceInfo.BrowserVersion,
                Platform = deviceInfo.Platform,
                OperatingSystem = deviceInfo.OperatingSystem,
                OsVersion = deviceInfo.OsVersion,
            };
        }

        private async Task<TokenEntity> GetAssignedToken(UserEntity user, DeviceInfo deviceInfo, bool isRefreshToken = false)
        {
            var device = await _deviceRepository.GetByDeviceIdentifierAsync(deviceInfo.DeviceIdentifier, user.Id);

            if (device is null || device.UserId != user.Id)
            {
                device = GenerateDevice(deviceInfo);
                user.Devices.Add(device);
            }
            if (isRefreshToken || device.Token is null || device.Token.AccessTokenExpiresAt < DateTime.UtcNow)
            {
                var newTokenEntity = await GenerateNewToken(user);
                device.Token = newTokenEntity;
                user.Tokens.Add(newTokenEntity);
            }
            if (user.Devices.Count > _jwtSettings.MaxAllowedDevices)
            {
                var oldDevice = user.Devices.MinBy(t => t.CreatedAt)!;
                var oldToken = await _tokenRepository.GetByIdAsync(oldDevice.TokenId);
                if (oldToken != null) user.Tokens.Remove(oldToken);
                user.Devices.Remove(oldDevice);
            }
            await _userRepository.UpdateAsync(user);

            return device.Token;
        }

        private async Task<TokenEntity> GenerateNewToken(UserEntity user)
        {
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var accessTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresMinutes);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            var newTokenEntity = new TokenEntity
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiryTime,
                RefreshTokenExpiresAt = refreshTokenExpiryTime,
            };

            return newTokenEntity;
        }
    }
}
