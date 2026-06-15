namespace Sistema_de_chamados.src.API.DTOs
{
    public class AcompanhamentoCriarDTO
    {
        public int ChamadoId { get; set; }
        public int ResponsavelId { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
