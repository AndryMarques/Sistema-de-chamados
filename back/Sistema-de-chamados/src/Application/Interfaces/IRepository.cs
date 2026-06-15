using System.Linq.Expressions;

namespace Sistema_de_chamados.src.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> ObterPorIdAsync(int id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<IEnumerable<T>> ObterAsync(Expression<Func<T, bool>> predicate);
        Task<T?> ObterUmAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExisteAsync(Expression<Func<T, bool>> predicate);
        Task<int> ContarAsync(Expression<Func<T, bool>>? predicate = null);
        Task AdicionarAsync(T entity);
        Task AdicionarRangeAsync(IEnumerable<T> entities);
        void Atualizar(T entity);
        void AtualizarRange(IEnumerable<T> entities);
        void Remover(T entity);
        void RemoverRange(IEnumerable<T> entities);
        Task SalvarAsync();
    }
}
