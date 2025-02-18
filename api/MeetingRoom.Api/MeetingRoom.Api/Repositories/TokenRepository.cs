using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TokenEntity?> GetByIdAsync(string id)
        {
            return await _context.Tokens.FindAsync(id);
        }

        public async Task<TokenEntity> CreateAsync(TokenEntity tokenEntity)
        {
            _context.Tokens.Add(tokenEntity);
            await _context.SaveChangesAsync();
            return tokenEntity;
        }

        public async Task UpdateAsync(TokenEntity tokenEntity)
        {
            _context.Entry(tokenEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var tokenEntity = await _context.Tokens.FindAsync(id);
            if (tokenEntity != null)
            {
                _context.Tokens.Remove(tokenEntity);
            }
        }

        public async Task RemoveUserIdAsync()
        {
            _context.Tokens.RemoveRange(_context.Tokens);
        }
    }
}
