using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.API.DTOs
{
    public class ChamadoAtualizarDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public ChamadoPrioridade Prioridade { get; set; }
        public ChamadoStatus Status { get; set; }
        public int? ResponsavelId { get; set; }
    }
}
