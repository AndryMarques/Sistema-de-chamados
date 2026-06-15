using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IResponsavelService
    {
        Task<ResponsavelResponseDTO> CriarAsync(ResponsavelCriarDTO responsavelDTO);
        Task<ResponsavelResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<ResponsavelResponseDTO>> ObterTodosAsync();
        Task DeletarAsync(int id);
        Task AtribuirChamadoAsync(int chamadoId);
    }
}
