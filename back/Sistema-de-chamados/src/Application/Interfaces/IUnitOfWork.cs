namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        IChamadoRepository Chamados { get; }
        IResponsavelRepository Responsaveis { get; }
        IAcompanhamentoRepository Acompanhamentos { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }
}
