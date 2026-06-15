using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;

namespace Sistema_de_chamados.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResponsaveisController : ControllerBase
    {
        private readonly IResponsavelService _responsavelService;

        public ResponsaveisController(IResponsavelService responsavelService)
        {
            _responsavelService = responsavelService;
        }

        /// <summary>
        /// Cria um novo responsável a partir de um usuário existente
        /// </summary>
        /// <param name="responsavelDTO">Dados do novo responsável</param>
        /// <returns>Responsável criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponsavelResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Criar([FromBody] ResponsavelCriarDTO responsavelDTO)
        {
            try
            {
                var responsavelCriado = await _responsavelService.CriarAsync(responsavelDTO);
                return CreatedAtAction(nameof(ObterPorId), new { id = responsavelCriado.Id }, responsavelCriado);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                {
                    return NotFound(new { message = ex.Message });
                }
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar responsável", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os responsáveis ordenados por carga de trabalho
        /// </summary>
        /// <returns>Lista de responsáveis</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ResponsavelResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var responsaveis = await _responsavelService.ObterTodosAsync();
                return Ok(responsaveis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter responsáveis", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um responsável por ID com seus chamados
        /// </summary>
        /// <param name="id">ID do responsável</param>
        /// <returns>Dados do responsável com chamados</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponsavelResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var responsavel = await _responsavelService.ObterPorIdAsync(id);

                if (responsavel == null)
                {
                    return NotFound(new { message = $"Responsável com ID {id} não encontrado" });
                }

                return Ok(responsavel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter responsável", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um responsável (apenas sem chamados abertos)
        /// </summary>
        /// <param name="id">ID do responsável</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _responsavelService.DeletarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar responsável", error = ex.Message });
            }
        }

        /// <summary>
        /// Atribui um chamado automaticamente ao responsável com menor carga
        /// </summary>
        /// <param name="chamadoId">ID do chamado</param>
        /// <returns>Sem conteúdo</returns>
        [HttpPost("atribuir-chamado/{chamadoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtribuirChamado(int chamadoId)
        {
            try
            {
                await _responsavelService.AtribuirChamadoAsync(chamadoId);
                return Ok(new { message = "Chamado atribuído com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atribuir chamado", error = ex.Message });
            }
        }
    }
}
