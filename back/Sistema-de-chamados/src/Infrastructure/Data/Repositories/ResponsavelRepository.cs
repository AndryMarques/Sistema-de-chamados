using Microsoft.EntityFrameworkCore;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Infrastructure.Data.Context;

namespace Sistema_de_chamados.src.Infrastructure.Data.Repositories
{
    public class ResponsavelRepository : Repository<Responsavel>, IResponsavelRepository
    {
        public ResponsavelRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Responsavel?> ObterComChamadosAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Usuario)
                .Include(r => r.ChamadosAtribuidos)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Responsavel?> ObterPorUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Include(r => r.Usuario)
                .Include(r => r.ChamadosAtribuidos)
                .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Responsavel>> ObterComMenorCargaTrabalhoAsync()
        {
            return await _dbSet
                .Include(r => r.Usuario)
                .Include(r => r.ChamadosAtribuidos)
                .OrderBy(r => r.ChamadosEmAberto)
                .ToListAsync();
        }

        public async Task<Responsavel?> ObterResponsavelComMenorCargaAsync()
        {
            return await _dbSet
                .Include(r => r.Usuario)
                .Include(r => r.ChamadosAtribuidos)
                .OrderBy(r => r.ChamadosEmAberto)
                .FirstOrDefaultAsync();
        }
    }
}
