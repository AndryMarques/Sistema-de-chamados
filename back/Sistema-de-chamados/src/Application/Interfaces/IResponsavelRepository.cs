using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IResponsavelRepository : IRepository<Responsavel>
    {
        Task<Responsavel?> ObterComChamadosAsync(int id);
        Task<Responsavel?> ObterPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Responsavel>> ObterComMenorCargaTrabalhoAsync();
        Task<Responsavel?> ObterResponsavelComMenorCargaAsync();
    }
}
