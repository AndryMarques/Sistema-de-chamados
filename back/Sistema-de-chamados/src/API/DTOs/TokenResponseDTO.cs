namespace Sistema_de_chamados.src.API.DTOs
{
    public class TokenResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioResponseDTO? Usuario { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}
