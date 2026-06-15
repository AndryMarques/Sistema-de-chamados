namespace Sistema_de_chamados.src.API.DTOs
{
    public class UsuarioAtualizarDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
