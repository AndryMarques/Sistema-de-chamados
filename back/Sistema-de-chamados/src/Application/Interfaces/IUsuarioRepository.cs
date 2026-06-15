using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterComResponsaveisAsync(int id);
        Task<Usuario?> ObterComChamadosAsync(int id);
        Task<IEnumerable<Usuario>> ObterResponsaveisAsync();
    }
}
