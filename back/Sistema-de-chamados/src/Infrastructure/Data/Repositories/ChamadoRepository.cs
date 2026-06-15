using Microsoft.EntityFrameworkCore;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;
using Sistema_de_chamados.src.Infrastructure.Data.Context;

namespace Sistema_de_chamados.src.Infrastructure.Data.Repositories
{
    public class ChamadoRepository : Repository<Chamado>, IChamadoRepository
    {
        public ChamadoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Chamado?> ObterComAcompanhamentosAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Include(c => c.Acompanhamentos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Chamado>> ObterPorStatusAsync(ChamadoStatus status)
        {
            return await _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> ObterPorPrioridadeAsync(ChamadoPrioridade prioridade)
        {
            return await _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Where(c => c.Prioridade == prioridade)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> ObterPorResponsavelAsync(int responsavelId)
        {
            return await _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Where(c => c.ResponsavelId == responsavelId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> ObterPorUsuarioAsync(int usuarioId)
        {
            return await _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> ObterAbertosComFiltroAsync(ChamadoStatus? status = null, ChamadoPrioridade? prioridade = null)
        {
            var query = _dbSet
                .Include(c => c.Usuario)
                .Include(c => c.Responsavel)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (prioridade.HasValue)
                query = query.Where(c => c.Prioridade == prioridade.Value);

            return await query.ToListAsync();
        }

        public async Task<int> ContarChamadosAbertosPorResponsavelAsync(int responsavelId)
        {
            return await _dbSet
                .Where(c => c.ResponsavelId == responsavelId && 
                           (c.Status == ChamadoStatus.Aberto || c.Status == ChamadoStatus.EmAndamento))
                .CountAsync();
        }
    }
}
