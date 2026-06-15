using Microsoft.EntityFrameworkCore.Storage;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Infrastructure.Data.Context;

namespace Sistema_de_chamados.src.Infrastructure.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUsuarioRepository? _usuarioRepository;
        private IChamadoRepository? _chamadoRepository;
        private IResponsavelRepository? _responsavelRepository;
        private IAcompanhamentoRepository? _acompanhamentoRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUsuarioRepository Usuarios
        {
            get { return _usuarioRepository ??= new UsuarioRepository(_context); }
        }

        public IChamadoRepository Chamados
        {
            get { return _chamadoRepository ??= new ChamadoRepository(_context); }
        }

        public IResponsavelRepository Responsaveis
        {
            get { return _responsavelRepository ??= new ResponsavelRepository(_context); }
        }

        public IAcompanhamentoRepository Acompanhamentos
        {
            get { return _acompanhamentoRepository ??= new AcompanhamentoRepository(_context); }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
