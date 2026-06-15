using AutoMapper;
using Moq;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Application.Services;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.Tests.Services
{
    public class AcompanhamentoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IChamadoRepository> _chamadoRepoMock;
        private readonly Mock<IResponsavelRepository> _responsavelRepoMock;
        private readonly Mock<IAcompanhamentoRepository> _acompanhamentoRepoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly AcompanhamentoService _service;

        public AcompanhamentoServiceTests()
        {
            _chamadoRepoMock = new Mock<IChamadoRepository>();
            _responsavelRepoMock = new Mock<IResponsavelRepository>();
            _acompanhamentoRepoMock = new Mock<IAcompanhamentoRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.Chamados).Returns(_chamadoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Responsaveis).Returns(_responsavelRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Acompanhamentos).Returns(_acompanhamentoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);

            _service = new AcompanhamentoService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        // ── CriarAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_DeveCriarAcompanhamento_QuandoChamadoEResponsavelExistem()
        {
            var dto = new AcompanhamentoCriarDTO { ChamadoId = 1, ResponsavelId = 2, Descricao = "Analisando o problema" };
            var chamado = new Chamado { Id = 1 };
            var responsavel = new Responsavel { Id = 2 };
            var acompanhamento = new Acompanhamento { Id = 10, ChamadoId = 1, ResponsavelId = 2 };
            var responseDTO = new AcompanhamentoResponseDTO { Id = 10 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(responsavel);
            _mapperMock.Setup(m => m.Map<Acompanhamento>(dto)).Returns(acompanhamento);
            _mapperMock.Setup(m => m.Map<AcompanhamentoResponseDTO>(acompanhamento)).Returns(responseDTO);
            _acompanhamentoRepoMock.Setup(r => r.AdicionarAsync(acompanhamento)).Returns(Task.CompletedTask);
            _acompanhamentoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            _acompanhamentoRepoMock.Verify(r => r.AdicionarAsync(acompanhamento), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveDefinirDataAcompanhamento_AoCriar()
        {
            var dto = new AcompanhamentoCriarDTO { ChamadoId = 1, ResponsavelId = 2, Descricao = "Desc" };
            var chamado = new Chamado { Id = 1 };
            var responsavel = new Responsavel { Id = 2 };
            var acompanhamento = new Acompanhamento { Id = 1 };
            var responseDTO = new AcompanhamentoResponseDTO { Id = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(responsavel);
            _mapperMock.Setup(m => m.Map<Acompanhamento>(dto)).Returns(acompanhamento);
            _mapperMock.Setup(m => m.Map<AcompanhamentoResponseDTO>(acompanhamento)).Returns(responseDTO);
            _acompanhamentoRepoMock.Setup(r => r.AdicionarAsync(acompanhamento)).Returns(Task.CompletedTask);
            _acompanhamentoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.CriarAsync(dto);

            Assert.NotEqual(default, acompanhamento.DataAcompanhamento);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoChamadoNaoEncontrado()
        {
            var dto = new AcompanhamentoCriarDTO { ChamadoId = 99, ResponsavelId = 1 };
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Chamado?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Contains("Chamado", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoResponsavelNaoEncontrado()
        {
            var dto = new AcompanhamentoCriarDTO { ChamadoId = 1, ResponsavelId = 99 };
            var chamado = new Chamado { Id = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Responsavel?)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Contains("Responsável", ex.Message);
        }

        // ── ObterPorIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarDTO_QuandoAcompanhamentoExiste()
        {
            var acompanhamento = new Acompanhamento { Id = 1, Descricao = "Revisando" };
            var responseDTO = new AcompanhamentoResponseDTO { Id = 1 };

            _acompanhamentoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(acompanhamento);
            _mapperMock.Setup(m => m.Map<AcompanhamentoResponseDTO>(acompanhamento)).Returns(responseDTO);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoAcompanhamentoNaoExiste()
        {
            _acompanhamentoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Acompanhamento?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.Null(resultado);
        }

        // ── ObterPorChamadoAsync ─────────────────────────────────────────

        [Fact]
        public async Task ObterPorChamadoAsync_DeveRetornarAcompanhamentos_QuandoChamadoExiste()
        {
            var chamado = new Chamado { Id = 1 };
            var acompanhamentos = new List<Acompanhamento> { new() { Id = 1 }, new() { Id = 2 } };
            var dtos = new List<AcompanhamentoResponseDTO> { new() { Id = 1 }, new() { Id = 2 } };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _acompanhamentoRepoMock.Setup(r => r.ObterPorChamadoAsync(1)).ReturnsAsync(acompanhamentos);
            _mapperMock.Setup(m => m.Map<IEnumerable<AcompanhamentoResponseDTO>>(acompanhamentos)).Returns(dtos);

            var resultado = await _service.ObterPorChamadoAsync(1);

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task ObterPorChamadoAsync_DeveLancarExcecao_QuandoChamadoNaoEncontrado()
        {
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Chamado?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ObterPorChamadoAsync(99));
        }

        // ── ObterPorResponsavelAsync ─────────────────────────────────────

        [Fact]
        public async Task ObterPorResponsavelAsync_DeveRetornarAcompanhamentos_QuandoResponsavelExiste()
        {
            var responsavel = new Responsavel { Id = 2 };
            var acompanhamentos = new List<Acompanhamento> { new() { Id = 1 } };
            var dtos = new List<AcompanhamentoResponseDTO> { new() { Id = 1 } };

            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(responsavel);
            _acompanhamentoRepoMock.Setup(r => r.ObterPorResponsavelAsync(2)).ReturnsAsync(acompanhamentos);
            _mapperMock.Setup(m => m.Map<IEnumerable<AcompanhamentoResponseDTO>>(acompanhamentos)).Returns(dtos);

            var resultado = await _service.ObterPorResponsavelAsync(2);

            Assert.Single(resultado);
        }

        [Fact]
        public async Task ObterPorResponsavelAsync_DeveLancarExcecao_QuandoResponsavelNaoEncontrado()
        {
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Responsavel?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ObterPorResponsavelAsync(99));
        }

        // ── DeletarAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task DeletarAsync_DeveDeletar_QuandoAcompanhamentoExiste()
        {
            var acompanhamento = new Acompanhamento { Id = 1 };

            _acompanhamentoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(acompanhamento);
            _acompanhamentoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.DeletarAsync(1);

            _acompanhamentoRepoMock.Verify(r => r.Remover(acompanhamento), Times.Once);
            _acompanhamentoRepoMock.Verify(r => r.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoAcompanhamentoNaoEncontrado()
        {
            _acompanhamentoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Acompanhamento?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(99));
        }
    }
}
