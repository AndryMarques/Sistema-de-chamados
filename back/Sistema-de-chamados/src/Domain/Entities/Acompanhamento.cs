namespace Sistema_de_chamados.src.Domain.Entities
{
    public class Acompanhamento
    {
        public int Id { get; set; }
        public int ChamadoId { get; set; }
        public Chamado? Chamado { get; set; }
        public int ResponsavelId { get; set; }
        public Responsavel? Responsavel { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAcompanhamento { get; set; } = DateTime.UtcNow;
    }
}
