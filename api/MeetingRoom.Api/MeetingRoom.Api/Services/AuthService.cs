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
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, ITokenRepository tokenRepository, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
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

            var tokenEntity = await GenerateNewToken(user);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = user.MapToModel()
            };
        }

        public async Task<AuthResult> LoginByUsernameAsync(string username, string password)
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

            var tokenEntity = await GenerateNewToken(user);

            return new AuthResult
            {
                AccessToken = tokenEntity.AccessToken,
                RefreshToken = tokenEntity.RefreshToken,
                ExpiresIn = tokenEntity.AccessTokenExpiresAt,
                User = user.MapToModel()
            };
        }

        public async Task<AuthResult> CreateAsync(UserRegisterRequest request, string operatorId)
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
            newUser.SetCreated(operatorId);
            await _userRepository.CreateAsync(newUser);

            var tokenEntity = await GenerateNewToken(newUser);

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

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.Tokens.First(t => t.RefreshToken == refreshToken).RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                throw new BusinessException("Invalid refresh token");
            }

            var tokenEntity = await GenerateNewToken(user);

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

        private async Task<TokenEntity> GenerateNewToken(UserEntity user)
        {
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var accessTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresMinutes);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            if (user.Tokens.Count > _jwtSettings.MaxAllowedDevices)
            {
                var old = user.Tokens.MinBy(t => t.CreatedAt)!;
                user.Tokens.Remove(old);
                //await _tokenRepository.DeleteAsync(old.Id);
            }

            var newTokenEntity = new TokenEntity
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiryTime,
                RefreshTokenExpiresAt = refreshTokenExpiryTime,
            };
            await _tokenRepository.CreateAsync(newTokenEntity);
            //user.Tokens.Add(newTokenEntity);
            await _userRepository.UpdateAsync(user);

            return newTokenEntity;
        }
    }
}
