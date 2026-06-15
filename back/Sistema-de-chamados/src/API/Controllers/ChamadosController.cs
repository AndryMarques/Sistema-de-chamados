using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChamadosController : ControllerBase
    {
        private readonly IChamadoService _chamadoService;
        private readonly IValidator<ChamadoCriarDTO> _chamadoCriarValidator;
        private readonly IValidator<ChamadoAtualizarDTO> _chamadoAtualizarValidator;

        public ChamadosController(
            IChamadoService chamadoService,
            IValidator<ChamadoCriarDTO> chamadoCriarValidator,
            IValidator<ChamadoAtualizarDTO> chamadoAtualizarValidator)
        {
            _chamadoService = chamadoService;
            _chamadoCriarValidator = chamadoCriarValidator;
            _chamadoAtualizarValidator = chamadoAtualizarValidator;
        }

        /// <summary>
        /// Cria um novo chamado
        /// </summary>
        /// <param name="chamadoCriarDTO">Dados do novo chamado</param>
        /// <returns>Chamado criado (distribuído automaticamente)</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ChamadoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Criar([FromBody] ChamadoCriarDTO chamadoCriarDTO)
        {
            try
            {
                // Validar DTO
                var validationResult = await _chamadoCriarValidator.ValidateAsync(chamadoCriarDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var chamadoCriado = await _chamadoService.CriarAsync(chamadoCriarDTO);

                return CreatedAtAction(nameof(ObterPorId), new { id = chamadoCriado.Id }, chamadoCriado);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar chamado", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os chamados
        /// </summary>
        /// <returns>Lista de chamados</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChamadoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var chamados = await _chamadoService.ObterTodosAsync();
                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter chamados", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um chamado por ID com acompanhamentos
        /// </summary>
        /// <param name="id">ID do chamado</param>
        /// <returns>Dados do chamado com acompanhamentos</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ChamadoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var chamado = await _chamadoService.ObterPorIdAsync(id);

                if (chamado == null)
                {
                    return NotFound(new { message = $"Chamado com ID {id} não encontrado" });
                }

                return Ok(chamado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter chamado", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém chamados por usuário
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Lista de chamados do usuário</returns>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<ChamadoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorUsuario(int usuarioId)
        {
            try
            {
                var chamados = await _chamadoService.ObterPorUsuarioAsync(usuarioId);
                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter chamados do usuário", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém chamados por responsável
        /// </summary>
        /// <param name="responsavelId">ID do responsável</param>
        /// <returns>Lista de chamados do responsável</returns>
        [HttpGet("responsavel/{responsavelId}")]
        [ProducesResponseType(typeof(IEnumerable<ChamadoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorResponsavel(int responsavelId)
        {
            try
            {
                var chamados = await _chamadoService.ObterPorResponsavelAsync(responsavelId);
                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter chamados do responsável", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um chamado
        /// </summary>
        /// <param name="id">ID do chamado</param>
        /// <param name="chamadoDTO">Dados para atualizar</param>
        /// <returns>Chamado atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ChamadoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ChamadoAtualizarDTO chamadoDTO)
        {
            try
            {
                if (id != chamadoDTO.Id)
                {
                    return BadRequest(new { message = "ID da URL não corresponde ao ID do corpo da requisição" });
                }

                // Validar DTO
                var validationResult = await _chamadoAtualizarValidator.ValidateAsync(chamadoDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var chamadoAtualizado = await _chamadoService.AtualizarAsync(chamadoDTO);

                return Ok(chamadoAtualizado);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar chamado", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um chamado (apenas fechados)
        /// </summary>
        /// <param name="id">ID do chamado</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _chamadoService.DeletarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar chamado", error = ex.Message });
            }
        }
    }
}
