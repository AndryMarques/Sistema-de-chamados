using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;

namespace Sistema_de_chamados.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AcompanhamentosController : ControllerBase
    {
        private readonly IAcompanhamentoService _acompanhamentoService;
        private readonly IValidator<AcompanhamentoCriarDTO> _acompanhamentoCriarValidator;

        public AcompanhamentosController(
            IAcompanhamentoService acompanhamentoService,
            IValidator<AcompanhamentoCriarDTO> acompanhamentoCriarValidator)
        {
            _acompanhamentoService = acompanhamentoService;
            _acompanhamentoCriarValidator = acompanhamentoCriarValidator;
        }

        /// <summary>
        /// Cria um novo acompanhamento para um chamado
        /// </summary>
        /// <param name="acompanhamentoDTO">Dados do acompanhamento</param>
        /// <returns>Acompanhamento criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AcompanhamentoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Criar([FromBody] AcompanhamentoCriarDTO acompanhamentoDTO)
        {
            try
            {
                // Validar DTO
                var validationResult = await _acompanhamentoCriarValidator.ValidateAsync(acompanhamentoDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var acompanhamentoCriado = await _acompanhamentoService.CriarAsync(acompanhamentoDTO);

                return CreatedAtAction(nameof(ObterPorId), new { id = acompanhamentoCriado.Id }, acompanhamentoCriado);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar acompanhamento", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um acompanhamento por ID
        /// </summary>
        /// <param name="id">ID do acompanhamento</param>
        /// <returns>Dados do acompanhamento</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AcompanhamentoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var acompanhamento = await _acompanhamentoService.ObterPorIdAsync(id);

                if (acompanhamento == null)
                {
                    return NotFound(new { message = $"Acompanhamento com ID {id} não encontrado" });
                }

                return Ok(acompanhamento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter acompanhamento", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os acompanhamentos de um chamado
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <returns>Lista de acompanhamentos ordenados por data (mais recente primeiro)</returns>
        [HttpGet("chamado/{chamadoId}")]
        [ProducesResponseType(typeof(IEnumerable<AcompanhamentoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorChamado(int chamadoId)
        {
            try
            {
                var acompanhamentos = await _acompanhamentoService.ObterPorChamadoAsync(chamadoId);
                return Ok(acompanhamentos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter acompanhamentos", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os acompanhamentos de um responsável
        /// </summary>
        /// <param name="responsavelId">ID do responsável</param>
        /// <returns>Lista de acompanhamentos do responsável</returns>
        [HttpGet("responsavel/{responsavelId}")]
        [ProducesResponseType(typeof(IEnumerable<AcompanhamentoResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorResponsavel(int responsavelId)
        {
            try
            {
                var acompanhamentos = await _acompanhamentoService.ObterPorResponsavelAsync(responsavelId);
                return Ok(acompanhamentos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter acompanhamentos", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um acompanhamento
        /// </summary>
        /// <param name="id">ID do acompanhamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _acompanhamentoService.DeletarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar acompanhamento", error = ex.Message });
            }
        }
    }
}
