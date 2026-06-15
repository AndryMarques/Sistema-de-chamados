namespace Sistema_de_chamados.src.API.DTOs
{
    public class ResponsavelResponseDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ChamadosEmAberto { get; set; }
        public DateTime DataAssociacao { get; set; }
        public UsuarioResponseDTO? Usuario { get; set; }
    }
}
