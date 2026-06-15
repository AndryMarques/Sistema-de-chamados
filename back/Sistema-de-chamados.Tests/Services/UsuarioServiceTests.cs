using AutoMapper;
using Moq;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Application.Services;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IChamadoRepository> _chamadoRepoMock;
        private readonly Mock<IResponsavelRepository> _responsavelRepoMock;
        private readonly Mock<IAcompanhamentoRepository> _acompanhamentoRepoMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _chamadoRepoMock = new Mock<IChamadoRepository>();
            _responsavelRepoMock = new Mock<IResponsavelRepository>();
            _acompanhamentoRepoMock = new Mock<IAcompanhamentoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Chamados).Returns(_chamadoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Responsaveis).Returns(_responsavelRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Acompanhamentos).Returns(_acompanhamentoRepoMock.Object);

            _service = new UsuarioService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        // ── ObterPorIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarDTO_QuandoUsuarioExiste()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria" };
            var responseDTO = new UsuarioResponseDTO { Id = 1, Nome = "Maria" };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _mapperMock.Setup(m => m.Map<UsuarioResponseDTO>(usuario)).Returns(responseDTO);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal("Maria", resultado.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoUsuarioNaoExiste()
        {
            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.Null(resultado);
        }

        // ── ObterTodosAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosOsUsuarios()
        {
            var usuarios = new List<Usuario> { new() { Id = 1 }, new() { Id = 2 }, new() { Id = 3 } };
            var dtos = new List<UsuarioResponseDTO> { new() { Id = 1 }, new() { Id = 2 }, new() { Id = 3 } };

            _usuarioRepoMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(usuarios);
            _mapperMock.Setup(m => m.Map<IEnumerable<UsuarioResponseDTO>>(usuarios)).Returns(dtos);

            var resultado = await _service.ObterTodosAsync();

            Assert.Equal(3, resultado.Count());
        }

        // ── AtualizarAsync ───────────────────────────────────────────────

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarDados_QuandoUsuarioExisteEEmailDisponivel()
        {
            var dto = new UsuarioAtualizarDTO { Id = 1, Nome = "João Atualizado", Email = "novo@email.com", Telefone = "11999", Ativo = true };
            var usuario = new Usuario { Id = 1, Nome = "João", Email = "antigo@email.com" };
            var responseDTO = new UsuarioResponseDTO { Id = 1, Nome = "João Atualizado" };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("novo@email.com")).ReturnsAsync((Usuario?)null);
            _usuarioRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UsuarioResponseDTO>(usuario)).Returns(responseDTO);

            var resultado = await _service.AtualizarAsync(dto);

            Assert.Equal("João Atualizado", usuario.Nome);
            Assert.Equal("novo@email.com", usuario.Email);
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_NaoDeveVerificarEmail_QuandoEmailNaoMudou()
        {
            var dto = new UsuarioAtualizarDTO { Id = 1, Nome = "João Atualizado", Email = "mesmo@email.com", Ativo = true };
            var usuario = new Usuario { Id = 1, Nome = "João", Email = "mesmo@email.com" };
            var responseDTO = new UsuarioResponseDTO { Id = 1 };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _usuarioRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UsuarioResponseDTO>(usuario)).Returns(responseDTO);

            await _service.AtualizarAsync(dto);

            _usuarioRepoMock.Verify(r => r.ObterPorEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoEmailJaEmUso()
        {
            var dto = new UsuarioAtualizarDTO { Id = 1, Email = "ocupado@email.com" };
            var usuario = new Usuario { Id = 1, Email = "antigo@email.com" };
            var outroUsuario = new Usuario { Id = 2, Email = "ocupado@email.com" };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("ocupado@email.com")).ReturnsAsync(outroUsuario);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(dto));
            Assert.Contains("Email já está em uso", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            var dto = new UsuarioAtualizarDTO { Id = 99 };
            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(dto));
        }

        // ── DeletarAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task DeletarAsync_DeveDeletar_QuandoUsuarioSemChamadosAbertos()
        {
            var usuario = new Usuario { Id = 1 };
            var chamados = new List<Chamado> { new() { Id = 1, Status = ChamadoStatus.Fechado } };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _chamadoRepoMock.Setup(r => r.ObterPorUsuarioAsync(1)).ReturnsAsync(chamados);
            _usuarioRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.DeletarAsync(1);

            _usuarioRepoMock.Verify(r => r.Remover(usuario), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveDeletar_QuandoUsuarioSemChamados()
        {
            var usuario = new Usuario { Id = 1 };
            var chamados = new List<Chamado>();

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _chamadoRepoMock.Setup(r => r.ObterPorUsuarioAsync(1)).ReturnsAsync(chamados);
            _usuarioRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.DeletarAsync(1);

            _usuarioRepoMock.Verify(r => r.Remover(usuario), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoUsuarioTemChamadosAbertos()
        {
            var usuario = new Usuario { Id = 1 };
            var chamados = new List<Chamado>
            {
                new() { Id = 1, Status = ChamadoStatus.Fechado },
                new() { Id = 2, Status = ChamadoStatus.EmAndamento }
            };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _chamadoRepoMock.Setup(r => r.ObterPorUsuarioAsync(1)).ReturnsAsync(chamados);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(1));
            Assert.Contains("chamados abertos", ex.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(99));
        }
    }
}
