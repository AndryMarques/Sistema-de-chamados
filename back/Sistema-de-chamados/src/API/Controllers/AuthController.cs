using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;

namespace Sistema_de_chamados.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<UsuarioCriarDTO> _usuarioCriarValidator;

        public AuthController(
            IAutenticacaoService autenticacaoService,
            IValidator<LoginDTO> loginValidator,
            IValidator<UsuarioCriarDTO> usuarioCriarValidator)
        {
            _autenticacaoService = autenticacaoService;
            _loginValidator = loginValidator;
            _usuarioCriarValidator = usuarioCriarValidator;
        }

        /// <summary>
        /// Realiza login e retorna token JWT
        /// </summary>
        /// <param name="loginDTO">Email e Senha do usuário</param>
        /// <returns>Token JWT e dados do usuário</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                // Validar DTO
                var validationResult = await _loginValidator.ValidateAsync(loginDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                // Tentar fazer login
                var tokenResponse = await _autenticacaoService.LoginAsync(loginDTO);

                if (tokenResponse == null)
                {
                    return Unauthorized(new { message = "Email ou senha inválidos" });
                }

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao realizar login", error = ex.Message });
            }
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="usuarioCriarDTO">Dados do novo usuário</param>
        /// <returns>Usuário criado</returns>
        [HttpPost("registrar")]
        [ProducesResponseType(typeof(UsuarioResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Registrar([FromBody] UsuarioCriarDTO usuarioCriarDTO)
        {
            try
            {
                // Validar DTO
                var validationResult = await _usuarioCriarValidator.ValidateAsync(usuarioCriarDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
                }

                // Registrar usuário
                var usuarioRegistrado = await _autenticacaoService.RegistrarAsync(usuarioCriarDTO);

                return CreatedAtAction(nameof(Registrar), new { id = usuarioRegistrado.Id }, usuarioRegistrado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao registrar usuário", error = ex.Message });
            }
        }
    }
}
