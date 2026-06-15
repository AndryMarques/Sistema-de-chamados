using Microsoft.EntityFrameworkCore;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Infrastructure.Data.Context;

namespace Sistema_de_chamados.src.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> ObterComResponsaveisAsync(int id)
        {
            return await _dbSet
                .Include(u => u.ResponsaveisAssociados)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObterComChamadosAsync(int id)
        {
            return await _dbSet
                .Include(u => u.ChamadosAbertos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Usuario>> ObterResponsaveisAsync()
        {
            return await _dbSet
                .Include(u => u.ResponsaveisAssociados)
                .Where(u => u.ResponsaveisAssociados.Any())
                .ToListAsync();
        }
    }
}
