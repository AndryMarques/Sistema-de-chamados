using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.API.DTOs
{
    public class ChamadoCriarDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public ChamadoPrioridade Prioridade { get; set; } = ChamadoPrioridade.Media;
        public int UsuarioId { get; set; }
    }
}
