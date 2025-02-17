using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Repositories;
using MeetingRoom.Api.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace MeetingRoom.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new BusinessException("Invalid email or password");
            }

            if (user.Status == UserStatus.Disabled)
            {
                throw new BusinessException("User account is disabled");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _userRepository.UpdateAsync(user);

            return new AuthResult
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = refreshTokenExpiryTime,
                User = UserService.MapToDto(user)
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

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var accessTokenExpiryTime = DateTime.UtcNow.AddMinutes(30);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.Tokens.Add(new TokenEntity
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiryTime,
                RefreshTokenExpiresAt = refreshTokenExpiryTime,
            });
            await _userRepository.UpdateAsync(user);

            return new AuthResult
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = refreshTokenExpiryTime,
                User = UserService.MapToDto(user)
            };
        }

        public async Task<AuthResult> CreateAsync(IdentityUser user, string password)
        {
            var existedUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existedUser != null)
            {
                throw new BusinessException("User already existed");
            }

            var userEntity = new UserEntity
            {
                Username = user.UserName,
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            var token = GenerateJwtToken(userEntity);
            var refreshToken = GenerateRefreshToken();
            var accessTokenExpiryTime = DateTime.UtcNow.AddMinutes(30);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            userEntity.Tokens.Add(new TokenEntity
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiryTime,
                RefreshTokenExpiresAt = refreshTokenExpiryTime,
            });
            await _userRepository.UpdateAsync(userEntity);

            return new AuthResult
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = refreshTokenExpiryTime,
                User = UserService.MapToDto(user)
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
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new BusinessException("Invalid refresh token");
            }

            var token = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _userRepository.UpdateAsync(user);

            return new AuthResult
            {
                AccessToken = token,
                RefreshToken = newRefreshToken,
                ExpiresIn = refreshTokenExpiryTime,
                User = UserService.MapToDto(user)
            };
        }

        public async Task<bool> LogoutAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
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
                    new Claim(ClaimTypes.Email, userEntity.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, userEntity.Role.ToString()),
                    new Claim("id", userEntity.Id.ToString())
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
    }
}
