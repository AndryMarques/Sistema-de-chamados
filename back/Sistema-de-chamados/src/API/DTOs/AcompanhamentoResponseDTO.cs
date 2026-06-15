namespace Sistema_de_chamados.src.API.DTOs
{
    public class AcompanhamentoResponseDTO
    {
        public int Id { get; set; }
        public int ChamadoId { get; set; }
        public int ResponsavelId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAcompanhamento { get; set; }
        public ResponsavelResponseDTO? Responsavel { get; set; }
    }
}
