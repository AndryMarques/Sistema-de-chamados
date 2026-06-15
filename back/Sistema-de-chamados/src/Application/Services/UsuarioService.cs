using AutoMapper;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UsuarioResponseDTO?> ObterPorIdAsync(int id)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(id);

                if (usuario == null)
                {
                    return null;
                }

                return _mapper.Map<UsuarioResponseDTO>(usuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> ObterTodosAsync()
        {
            try
            {
                var usuarios = await _unitOfWork.Usuarios.ObterTodosAsync();
                return _mapper.Map<IEnumerable<UsuarioResponseDTO>>(usuarios);
            }
            catch
            {
                throw;
            }
        }

        public async Task<UsuarioResponseDTO> AtualizarAsync(UsuarioAtualizarDTO usuarioDTO)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(usuarioDTO.Id);

                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com ID {usuarioDTO.Id} não encontrado");
                }

                // Verificar se o novo email já existe para outro usuário
                if (usuario.Email != usuarioDTO.Email)
                {
                    var usuarioComEmailExistente = await _unitOfWork.Usuarios.ObterPorEmailAsync(usuarioDTO.Email);

                    if (usuarioComEmailExistente != null)
                    {
                        throw new InvalidOperationException("Email já está em uso por outro usuário");
                    }
                }

                // Atualizar dados
                usuario.Nome = usuarioDTO.Nome;
                usuario.Email = usuarioDTO.Email;
                usuario.Telefone = usuarioDTO.Telefone;
                usuario.Ativo = usuarioDTO.Ativo;
                usuario.DataAtualizacao = DateTime.UtcNow;

                _unitOfWork.Usuarios.Atualizar(usuario);
                await _unitOfWork.Usuarios.SalvarAsync();

                return _mapper.Map<UsuarioResponseDTO>(usuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeletarAsync(int id)
        {
            try
            {
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(id);

                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com ID {id} não encontrado");
                }

                // Verificar se o usuário tem chamados abertos
                var chamadosAbertos = await _unitOfWork.Chamados.ObterPorUsuarioAsync(id);

                if (chamadosAbertos.Any(c => c.Status != Domain.Enums.ChamadoStatus.Fechado))
                {
                    throw new InvalidOperationException("Não é possível deletar usuário com chamados abertos");
                }

                _unitOfWork.Usuarios.Remover(usuario);
                await _unitOfWork.Usuarios.SalvarAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
