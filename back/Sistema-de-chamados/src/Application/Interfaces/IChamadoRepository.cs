using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IChamadoRepository : IRepository<Chamado>
    {
        Task<Chamado?> ObterComAcompanhamentosAsync(int id);
        Task<IEnumerable<Chamado>> ObterPorStatusAsync(ChamadoStatus status);
        Task<IEnumerable<Chamado>> ObterPorPrioridadeAsync(ChamadoPrioridade prioridade);
        Task<IEnumerable<Chamado>> ObterPorResponsavelAsync(int responsavelId);
        Task<IEnumerable<Chamado>> ObterPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Chamado>> ObterAbertosComFiltroAsync(ChamadoStatus? status = null, ChamadoPrioridade? prioridade = null);
        Task<int> ContarChamadosAbertosPorResponsavelAsync(int responsavelId);
    }
}
