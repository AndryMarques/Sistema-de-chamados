using AutoMapper;
using Moq;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Application.Services;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.Tests.Services
{
    public class ResponsavelServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IResponsavelRepository> _responsavelRepoMock;
        private readonly Mock<IChamadoRepository> _chamadoRepoMock;
        private readonly Mock<IAcompanhamentoRepository> _acompanhamentoRepoMock;
        private readonly ResponsavelService _service;

        public ResponsavelServiceTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _responsavelRepoMock = new Mock<IResponsavelRepository>();
            _chamadoRepoMock = new Mock<IChamadoRepository>();
            _acompanhamentoRepoMock = new Mock<IAcompanhamentoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Responsaveis).Returns(_responsavelRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Chamados).Returns(_chamadoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Acompanhamentos).Returns(_acompanhamentoRepoMock.Object);

            _service = new ResponsavelService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        // ── CriarAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_DeveCriarResponsavel_QuandoUsuarioExisteENaoEResponsavel()
        {
            var dto = new ResponsavelCriarDTO { UsuarioId = 1 };
            var usuario = new Usuario { Id = 1 };
            var responsavel = new Responsavel { Id = 10, UsuarioId = 1, ChamadosEmAberto = 0 };
            var responseDTO = new ResponsavelResponseDTO { Id = 10 };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _responsavelRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(1)).ReturnsAsync((Responsavel?)null);
            _responsavelRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Responsavel>())).Returns(Task.CompletedTask);
            _responsavelRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(It.IsAny<int>())).ReturnsAsync(responsavel);
            _mapperMock.Setup(m => m.Map<ResponsavelResponseDTO>(responsavel)).Returns(responseDTO);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            _responsavelRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Responsavel>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            var dto = new ResponsavelCriarDTO { UsuarioId = 99 };
            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoUsuarioJaEResponsavel()
        {
            var dto = new ResponsavelCriarDTO { UsuarioId = 1 };
            var usuario = new Usuario { Id = 1 };
            var responsavelExistente = new Responsavel { Id = 5, UsuarioId = 1 };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _responsavelRepoMock.Setup(r => r.ObterPorUsuarioIdAsync(1)).ReturnsAsync(responsavelExistente);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Contains("já é um responsável", ex.Message);
        }

        // ── ObterPorIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarDTO_QuandoResponsavelExiste()
        {
            var responsavel = new Responsavel { Id = 1 };
            var responseDTO = new ResponsavelResponseDTO { Id = 1 };

            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(1)).ReturnsAsync(responsavel);
            _mapperMock.Setup(m => m.Map<ResponsavelResponseDTO>(responsavel)).Returns(responseDTO);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoResponsavelNaoExiste()
        {
            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(99)).ReturnsAsync((Responsavel?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.Null(resultado);
        }

        // ── ObterTodosAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarOrdenadoPorCarga()
        {
            var responsaveis = new List<Responsavel>
            {
                new() { Id = 1, ChamadosEmAberto = 0 },
                new() { Id = 2, ChamadosEmAberto = 3 }
            };
            var dtos = new List<ResponsavelResponseDTO> { new() { Id = 1 }, new() { Id = 2 } };

            _responsavelRepoMock.Setup(r => r.ObterComMenorCargaTrabalhoAsync()).ReturnsAsync(responsaveis);
            _mapperMock.Setup(m => m.Map<IEnumerable<ResponsavelResponseDTO>>(responsaveis)).Returns(dtos);

            var resultado = await _service.ObterTodosAsync();

            Assert.Equal(2, resultado.Count());
        }

        // ── DeletarAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task DeletarAsync_DeveDeletar_QuandoResponsavelSemChamadosAbertos()
        {
            var responsavel = new Responsavel { Id = 1, ChamadosEmAberto = 0 };

            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(1)).ReturnsAsync(responsavel);
            _responsavelRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.DeletarAsync(1);

            _responsavelRepoMock.Verify(r => r.Remover(responsavel), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoTemChamadosAbertos()
        {
            var responsavel = new Responsavel { Id = 1, ChamadosEmAberto = 3 };
            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(1)).ReturnsAsync(responsavel);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(1));
            Assert.Contains("chamados abertos", ex.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoResponsavelNaoEncontrado()
        {
            _responsavelRepoMock.Setup(r => r.ObterComChamadosAsync(99)).ReturnsAsync((Responsavel?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(99));
        }

        // ── AtribuirChamadoAsync ─────────────────────────────────────────

        [Fact]
        public async Task AtribuirChamadoAsync_DeveAtribuirAoResponsavelComMenorCarga()
        {
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.Aberto, ResponsavelId = null };
            var responsavel = new Responsavel { Id = 2, ChamadosEmAberto = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterResponsavelComMenorCargaAsync()).ReturnsAsync(responsavel);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.AtribuirChamadoAsync(1);

            Assert.Equal(2, chamado.ResponsavelId);
            Assert.Equal(2, responsavel.ChamadosEmAberto);
        }

        [Fact]
        public async Task AtribuirChamadoAsync_DeveLiberarResponsavelAnterior_QuandoChamadoJaTinhaResponsavel()
        {
            var responsavelAnterior = new Responsavel { Id = 1, ChamadosEmAberto = 3 };
            var novoResponsavel = new Responsavel { Id = 2, ChamadosEmAberto = 1 };
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.Aberto, ResponsavelId = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(responsavelAnterior);
            _responsavelRepoMock.Setup(r => r.ObterResponsavelComMenorCargaAsync()).ReturnsAsync(novoResponsavel);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.AtribuirChamadoAsync(1);

            Assert.Equal(2, responsavelAnterior.ChamadosEmAberto);
            Assert.Equal(2, novoResponsavel.ChamadosEmAberto);
            Assert.Equal(2, chamado.ResponsavelId);
        }

        [Fact]
        public async Task AtribuirChamadoAsync_DeveLancarExcecao_QuandoChamadoNaoEncontrado()
        {
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Chamado?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtribuirChamadoAsync(99));
        }

        [Fact]
        public async Task AtribuirChamadoAsync_DeveLancarExcecao_QuandoNenhumResponsavelDisponivel()
        {
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.Aberto };
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterResponsavelComMenorCargaAsync()).ReturnsAsync((Responsavel?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtribuirChamadoAsync(1));
            Assert.Contains("Nenhum responsável", ex.Message);
        }
    }
}
