namespace Sistema_de_chamados.src.Domain.Entities
{
    public class Responsavel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int ChamadosEmAberto { get; set; } = 0;
        public DateTime DataAssociacao { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<Chamado> ChamadosAtribuidos { get; set; } = new List<Chamado>();
    }
}
