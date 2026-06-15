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
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IValidator<UsuarioAtualizarDTO> _usuarioAtualizarValidator;

        public UsuariosController(
            IUsuarioService usuarioService,
            IValidator<UsuarioAtualizarDTO> usuarioAtualizarValidator)
        {
            _usuarioService = usuarioService;
            _usuarioAtualizarValidator = usuarioAtualizarValidator;
        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var usuarios = await _usuarioService.ObterTodosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter usuários", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObterPorIdAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { message = $"Usuário com ID {id} não encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter usuário", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="usuarioDTO">Dados para atualizar</param>
        /// <returns>Usuário atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioAtualizarDTO usuarioDTO)
        {
            try
            {
                if (id != usuarioDTO.Id)
                {
                    return BadRequest(new { message = "ID da URL não corresponde ao ID do corpo da requisição" });
                }

                // Validar DTO
                var validationResult = await _usuarioAtualizarValidator.ValidateAsync(usuarioDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                var usuarioAtualizado = await _usuarioService.AtualizarAsync(usuarioDTO);

                return Ok(usuarioAtualizado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar usuário", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _usuarioService.DeletarAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar usuário", error = ex.Message });
            }
        }
    }
}
