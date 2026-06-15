using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IAcompanhamentoRepository : IRepository<Acompanhamento>
    {
        Task<IEnumerable<Acompanhamento>> ObterPorChamadoAsync(int chamadoId);
        Task<IEnumerable<Acompanhamento>> ObterPorResponsavelAsync(int responsavelId);
    }
}
