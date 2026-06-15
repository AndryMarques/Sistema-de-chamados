using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.Domain.Entities
{
    public class Chamado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public ChamadoPrioridade Prioridade { get; set; } = ChamadoPrioridade.Media;
        public ChamadoStatus Status { get; set; } = ChamadoStatus.Aberto;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int? ResponsavelId { get; set; }
        public Responsavel? Responsavel { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime? DataResolucao { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        // Relacionamentos
        public ICollection<Acompanhamento> Acompanhamentos { get; set; } = new List<Acompanhamento>();
    }
}
