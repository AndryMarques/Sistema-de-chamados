using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<UsuarioResponseDTO>> ObterTodosAsync();
        Task<UsuarioResponseDTO> AtualizarAsync(UsuarioAtualizarDTO usuarioDTO);
        Task DeletarAsync(int id);
    }
}
