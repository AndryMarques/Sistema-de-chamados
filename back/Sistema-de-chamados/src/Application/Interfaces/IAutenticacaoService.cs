using Sistema_de_chamados.src.API.DTOs;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<TokenResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<UsuarioResponseDTO?> RegistrarAsync(UsuarioCriarDTO usuarioCriarDTO);
        string GerarToken(UsuarioResponseDTO usuario);
    }
}
