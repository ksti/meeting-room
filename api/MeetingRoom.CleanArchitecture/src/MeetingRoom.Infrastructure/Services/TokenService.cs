using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Infrastructure.Settings;

namespace MeetingRoom.Infrastructure.Services;

/// <summary>
/// 令牌服务实现
/// 用于生成和验证JWT令牌
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    /// <summary>
    /// 生成访问令牌和刷新令牌
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>访问令牌、刷新令牌和过期时间（秒）</returns>
    public async Task<(string accessToken, string refreshToken, int expiresIn)> GenerateTokensAsync(User user)
    {
        // 创建声明
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        
        // 添加自定义声明
        if (user.Name != null)
        {
            claims.Add(new Claim("fullName", $"{user.Name.FirstName} {user.Name.LastName}"));
        }
        
        // 创建安全密钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 设置令牌过期时间
        var expiresIn = _jwtSettings.ExpirationInMinutes * 60; // 转换为秒
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        
        // 创建JWT令牌
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        
        // 生成访问令牌
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        // 生成刷新令牌
        var refreshToken = GenerateRefreshToken();
        
        return (accessToken, refreshToken, expiresIn);
    }
    
    /// <summary>
    /// 验证访问令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>是否有效</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的访问令牌、刷新令牌和过期时间（秒），如果刷新令牌无效则返回null</returns>
    public async Task<(string accessToken, string refreshToken, int expiresIn)?> RefreshTokenAsync(string refreshToken)
    {
        // 此处应该实现刷新令牌的验证逻辑
        // 在实际应用中，应该从数据库中查询刷新令牌是否存在且有效
        // 这里简化处理，直接返回null表示无效
        return null;
    }
    
    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
