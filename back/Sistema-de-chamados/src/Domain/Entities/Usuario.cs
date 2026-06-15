namespace Sistema_de_chamados.src.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        // Relacionamentos
        public ICollection<Chamado> ChamadosAbertos { get; set; } = new List<Chamado>();
        public ICollection<Responsavel> ResponsaveisAssociados { get; set; } = new List<Responsavel>();
    }
}
