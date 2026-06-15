using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IAcompanhamentoService
    {
        Task<AcompanhamentoResponseDTO> CriarAsync(AcompanhamentoCriarDTO acompanhamentoDTO);
        Task<AcompanhamentoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<AcompanhamentoResponseDTO>> ObterPorChamadoAsync(int chamadoId);
        Task<IEnumerable<AcompanhamentoResponseDTO>> ObterPorResponsavelAsync(int responsavelId);
        Task DeletarAsync(int id);
    }
}
