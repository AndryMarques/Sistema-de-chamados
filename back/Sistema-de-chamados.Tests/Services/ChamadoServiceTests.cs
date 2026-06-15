using AutoMapper;
using Moq;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Application.Services;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.Tests.Services
{
    public class ChamadoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IChamadoRepository> _chamadoRepoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IResponsavelRepository> _responsavelRepoMock;
        private readonly Mock<IAcompanhamentoRepository> _acompanhamentoRepoMock;
        private readonly ChamadoService _service;

        public ChamadoServiceTests()
        {
            _chamadoRepoMock = new Mock<IChamadoRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _responsavelRepoMock = new Mock<IResponsavelRepository>();
            _acompanhamentoRepoMock = new Mock<IAcompanhamentoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.Chamados).Returns(_chamadoRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Responsaveis).Returns(_responsavelRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Acompanhamentos).Returns(_acompanhamentoRepoMock.Object);

            _service = new ChamadoService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        // ── CriarAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task CriarAsync_DeveRetornarDTO_QuandoUsuarioExisteEResponsavelDisponivel()
        {
            var dto = new ChamadoCriarDTO { Titulo = "Teste", Descricao = "Desc", UsuarioId = 1, Prioridade = ChamadoPrioridade.Alta };
            var usuario = new Usuario { Id = 1, Nome = "João" };
            var responsavel = new Responsavel { Id = 2, ChamadosEmAberto = 1 };
            var chamado = new Chamado { Id = 10, Titulo = "Teste" };
            var responseDTO = new ChamadoResponseDTO { Id = 10, Titulo = "Teste" };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _responsavelRepoMock.Setup(r => r.ObterResponsavelComMenorCargaAsync()).ReturnsAsync(responsavel);
            _mapperMock.Setup(m => m.Map<Chamado>(dto)).Returns(chamado);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);
            _chamadoRepoMock.Setup(r => r.AdicionarAsync(chamado)).Returns(Task.CompletedTask);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Id);
            Assert.Equal(2, chamado.ResponsavelId);
            Assert.Equal(2, responsavel.ChamadosEmAberto);
            _chamadoRepoMock.Verify(r => r.AdicionarAsync(chamado), Times.Once);
            _chamadoRepoMock.Verify(r => r.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveCriarSemResponsavel_QuandoNenhumResponsavelDisponivel()
        {
            var dto = new ChamadoCriarDTO { Titulo = "Teste", UsuarioId = 1 };
            var usuario = new Usuario { Id = 1 };
            var chamado = new Chamado { Id = 5 };
            var responseDTO = new ChamadoResponseDTO { Id = 5 };

            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(usuario);
            _responsavelRepoMock.Setup(r => r.ObterResponsavelComMenorCargaAsync()).ReturnsAsync((Responsavel?)null);
            _mapperMock.Setup(m => m.Map<Chamado>(dto)).Returns(chamado);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);
            _chamadoRepoMock.Setup(r => r.AdicionarAsync(chamado)).Returns(Task.CompletedTask);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            var resultado = await _service.CriarAsync(dto);

            Assert.Null(chamado.ResponsavelId);
            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            var dto = new ChamadoCriarDTO { UsuarioId = 99 };
            _usuarioRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        }

        // ── ObterPorIdAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarDTO_QuandoChamadoExiste()
        {
            var chamado = new Chamado { Id = 1, Titulo = "Chamado 1" };
            var responseDTO = new ChamadoResponseDTO { Id = 1, Titulo = "Chamado 1" };

            _chamadoRepoMock.Setup(r => r.ObterComAcompanhamentosAsync(1)).ReturnsAsync(chamado);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);

            var resultado = await _service.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoChamadoNaoExiste()
        {
            _chamadoRepoMock.Setup(r => r.ObterComAcompanhamentosAsync(99)).ReturnsAsync((Chamado?)null);

            var resultado = await _service.ObterPorIdAsync(99);

            Assert.Null(resultado);
        }

        // ── ObterTodosAsync ──────────────────────────────────────────────

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaDeChamados()
        {
            var chamados = new List<Chamado> { new() { Id = 1 }, new() { Id = 2 } };
            var dtos = new List<ChamadoResponseDTO> { new() { Id = 1 }, new() { Id = 2 } };

            _chamadoRepoMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(chamados);
            _mapperMock.Setup(m => m.Map<IEnumerable<ChamadoResponseDTO>>(chamados)).Returns(dtos);

            var resultado = await _service.ObterTodosAsync();

            Assert.Equal(2, resultado.Count());
        }

        // ── AtualizarAsync ───────────────────────────────────────────────

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarStatus_QuandoChamadoExiste()
        {
            var dto = new ChamadoAtualizarDTO { Id = 1, Titulo = "Atualizado", Status = ChamadoStatus.EmAndamento, Prioridade = ChamadoPrioridade.Alta };
            var chamado = new Chamado { Id = 1, Titulo = "Original", Status = ChamadoStatus.Aberto };
            var responseDTO = new ChamadoResponseDTO { Id = 1, Titulo = "Atualizado" };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);

            var resultado = await _service.AtualizarAsync(dto);

            Assert.Equal("Atualizado", resultado.Titulo);
            Assert.Equal(ChamadoStatus.EmAndamento, chamado.Status);
        }

        [Fact]
        public async Task AtualizarAsync_DevePreencherDataResolucao_QuandoStatusMudaParaResolvido()
        {
            var dto = new ChamadoAtualizarDTO { Id = 1, Status = ChamadoStatus.Resolvido };
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.EmAndamento };
            var responseDTO = new ChamadoResponseDTO { Id = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);

            await _service.AtualizarAsync(dto);

            Assert.NotNull(chamado.DataResolucao);
        }

        [Fact]
        public async Task AtualizarAsync_DevePreencherDataEncerramento_QuandoStatusMudaParaFechado()
        {
            var responsavel = new Responsavel { Id = 3, ChamadosEmAberto = 2 };
            var dto = new ChamadoAtualizarDTO { Id = 1, Status = ChamadoStatus.Fechado };
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.EmAndamento, ResponsavelId = 3 };
            var responseDTO = new ChamadoResponseDTO { Id = 1 };

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _responsavelRepoMock.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync(responsavel);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ChamadoResponseDTO>(chamado)).Returns(responseDTO);

            await _service.AtualizarAsync(dto);

            Assert.NotNull(chamado.DataEncerramento);
            Assert.Equal(1, responsavel.ChamadosEmAberto);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoChamadoNaoEncontrado()
        {
            var dto = new ChamadoAtualizarDTO { Id = 99 };
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Chamado?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(dto));
        }

        // ── DeletarAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task DeletarAsync_DeveDeletar_QuandoChamadoEstaFechado()
        {
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.Fechado };
            var acompanhamentos = new List<Acompanhamento>();

            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);
            _acompanhamentoRepoMock.Setup(r => r.ObterPorChamadoAsync(1)).ReturnsAsync(acompanhamentos);
            _chamadoRepoMock.Setup(r => r.SalvarAsync()).Returns(Task.CompletedTask);

            await _service.DeletarAsync(1);

            _chamadoRepoMock.Verify(r => r.Remover(chamado), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoChamadoNaoEstaFechado()
        {
            var chamado = new Chamado { Id = 1, Status = ChamadoStatus.EmAndamento };
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(chamado);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(1));
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoChamadoNaoEncontrado()
        {
            _chamadoRepoMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Chamado?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeletarAsync(99));
        }

        // ── ObterPorUsuarioAsync / ObterPorResponsavelAsync ──────────────

        [Fact]
        public async Task ObterPorUsuarioAsync_DeveRetornarChamadosDoUsuario()
        {
            var chamados = new List<Chamado> { new() { Id = 1, UsuarioId = 5 } };
            var dtos = new List<ChamadoResponseDTO> { new() { Id = 1 } };

            _chamadoRepoMock.Setup(r => r.ObterPorUsuarioAsync(5)).ReturnsAsync(chamados);
            _mapperMock.Setup(m => m.Map<IEnumerable<ChamadoResponseDTO>>(chamados)).Returns(dtos);

            var resultado = await _service.ObterPorUsuarioAsync(5);

            Assert.Single(resultado);
        }

        [Fact]
        public async Task ObterPorResponsavelAsync_DeveRetornarChamadosDoResponsavel()
        {
            var chamados = new List<Chamado> { new() { Id = 1 }, new() { Id = 2 } };
            var dtos = new List<ChamadoResponseDTO> { new() { Id = 1 }, new() { Id = 2 } };

            _chamadoRepoMock.Setup(r => r.ObterPorResponsavelAsync(3)).ReturnsAsync(chamados);
            _mapperMock.Setup(m => m.Map<IEnumerable<ChamadoResponseDTO>>(chamados)).Returns(dtos);

            var resultado = await _service.ObterPorResponsavelAsync(3);

            Assert.Equal(2, resultado.Count());
        }
    }
}
