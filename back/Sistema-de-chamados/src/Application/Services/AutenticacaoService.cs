using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BCrypt.Net;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AutenticacaoService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<TokenResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                // Buscar usuário por email
                var usuario = await _unitOfWork.Usuarios.ObterPorEmailAsync(loginDTO.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Senha, usuario.Senha))
                {
                    return null;
                }

                if (!usuario.Ativo)
                {
                    return null;
                }

                // Mapear usuário para DTO
                var usuarioDTO = _mapper.Map<UsuarioResponseDTO>(usuario);

                // Gerar token
                var token = GerarToken(usuarioDTO);

                // Calcular expiração
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var expiracaoMinutos = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

                return new TokenResponseDTO
                {
                    Token = token,
                    Usuario = usuarioDTO,
                    ExpiresIn = DateTime.UtcNow.AddMinutes(expiracaoMinutos)
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UsuarioResponseDTO?> RegistrarAsync(UsuarioCriarDTO usuarioCriarDTO)
        {
            try
            {
                // Verificar se email já existe
                var usuarioExistente = await _unitOfWork.Usuarios.ObterPorEmailAsync(usuarioCriarDTO.Email);

                if (usuarioExistente != null)
                {
                    throw new InvalidOperationException("Email já está registrado");
                }

                // Criar novo usuário
                var usuario = _mapper.Map<Usuario>(usuarioCriarDTO);

                // Hash da senha
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuarioCriarDTO.Senha, workFactor: 12);
                usuario.DataCriacao = DateTime.UtcNow;
                usuario.Ativo = true;

                // Adicionar ao banco
                await _unitOfWork.Usuarios.AdicionarAsync(usuario);
                await _unitOfWork.Usuarios.SalvarAsync();

                // Mapear para DTO
                var usuarioDTO = _mapper.Map<UsuarioResponseDTO>(usuario);

                return usuarioDTO;
            }
            catch
            {
                throw;
            }
        }

        public string GerarToken(UsuarioResponseDTO usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "your-secret-key-here");
            var expiracaoMinutos = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Email, usuario.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiracaoMinutos),
                Issuer = "SistemaChamamdos",
                Audience = "SistemaChamamdosApp",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
