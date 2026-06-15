using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IChamadoService
    {
        Task<ChamadoResponseDTO> CriarAsync(ChamadoCriarDTO chamadoCriarDTO);
        Task<ChamadoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<ChamadoResponseDTO>> ObterTodosAsync();
        Task<ChamadoResponseDTO> AtualizarAsync(ChamadoAtualizarDTO chamadoDTO);
        Task DeletarAsync(int id);
        Task<IEnumerable<ChamadoResponseDTO>> ObterPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<ChamadoResponseDTO>> ObterPorResponsavelAsync(int responsavelId);
    }
}
