using Microsoft.EntityFrameworkCore;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Infrastructure.Data.Context;

namespace Sistema_de_chamados.src.Infrastructure.Data.Repositories
{
    public class AcompanhamentoRepository : Repository<Acompanhamento>, IAcompanhamentoRepository
    {
        public AcompanhamentoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Acompanhamento>> ObterPorChamadoAsync(int chamadoId)
        {
            return await _dbSet
                .Include(a => a.Chamado)
                .Include(a => a.Responsavel)
                .Where(a => a.ChamadoId == chamadoId)
                .OrderByDescending(a => a.DataAcompanhamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Acompanhamento>> ObterPorResponsavelAsync(int responsavelId)
        {
            return await _dbSet
                .Include(a => a.Chamado)
                .Include(a => a.Responsavel)
                .Where(a => a.ResponsavelId == responsavelId)
                .OrderByDescending(a => a.DataAcompanhamento)
                .ToListAsync();
        }
    }
}
