using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.API.DTOs
{
    public class ChamadoResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public ChamadoPrioridade Prioridade { get; set; }
        public ChamadoStatus Status { get; set; }
        public int UsuarioId { get; set; }
        public int? ResponsavelId { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataResolucao { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public UsuarioResponseDTO? Usuario { get; set; }
        public ResponsavelResponseDTO? Responsavel { get; set; }
        public IEnumerable<AcompanhamentoResponseDTO> Acompanhamentos { get; set; } = new List<AcompanhamentoResponseDTO>();
    }
}
