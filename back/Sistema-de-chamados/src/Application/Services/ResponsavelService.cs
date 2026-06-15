using AutoMapper;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.Application.Services
{
    public class ResponsavelService : IResponsavelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResponsavelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponsavelResponseDTO> CriarAsync(ResponsavelCriarDTO responsavelDTO)
        {
            try
            {
                // Verificar se usuário existe
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(responsavelDTO.UsuarioId);

                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com ID {responsavelDTO.UsuarioId} não encontrado");
                }

                // Verificar se usuário já é responsável
                var responsavelExistente = await _unitOfWork.Responsaveis.ObterPorUsuarioIdAsync(responsavelDTO.UsuarioId);

                if (responsavelExistente != null)
                {
                    throw new InvalidOperationException("Usuário já é um responsável");
                }

                // Criar novo responsável
                var responsavel = new Responsavel
                {
                    UsuarioId = responsavelDTO.UsuarioId,
                    ChamadosEmAberto = 0,
                    DataAssociacao = DateTime.UtcNow
                };

                await _unitOfWork.Responsaveis.AdicionarAsync(responsavel);
                await _unitOfWork.Responsaveis.SalvarAsync();

                // Mapear para DTO com relacionamento
                var responsavelComUsuario = await _unitOfWork.Responsaveis.ObterComChamadosAsync(responsavel.Id);
                return _mapper.Map<ResponsavelResponseDTO>(responsavelComUsuario);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponsavelResponseDTO?> ObterPorIdAsync(int id)
        {
            try
            {
                var responsavel = await _unitOfWork.Responsaveis.ObterComChamadosAsync(id);

                if (responsavel == null)
                {
                    return null;
                }

                return _mapper.Map<ResponsavelResponseDTO>(responsavel);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<ResponsavelResponseDTO>> ObterTodosAsync()
        {
            try
            {
                var responsaveis = await _unitOfWork.Responsaveis.ObterComMenorCargaTrabalhoAsync();
                return _mapper.Map<IEnumerable<ResponsavelResponseDTO>>(responsaveis);
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
                var responsavel = await _unitOfWork.Responsaveis.ObterComChamadosAsync(id);

                if (responsavel == null)
                {
                    throw new InvalidOperationException($"Responsável com ID {id} não encontrado");
                }

                // Verificar se tem chamados abertos
                if (responsavel.ChamadosEmAberto > 0)
                {
                    throw new InvalidOperationException("Não é possível deletar responsável com chamados abertos");
                }

                _unitOfWork.Responsaveis.Remover(responsavel);
                await _unitOfWork.Responsaveis.SalvarAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task AtribuirChamadoAsync(int chamadoId)
        {
            try
            {
                var chamado = await _unitOfWork.Chamados.ObterPorIdAsync(chamadoId);

                if (chamado == null)
                {
                    throw new InvalidOperationException($"Chamado com ID {chamadoId} não encontrado");
                }

                // Se já tem responsável, liberar da carga anterior
                if (chamado.ResponsavelId.HasValue)
                {
                    var responsavelAnterior = await _unitOfWork.Responsaveis.ObterPorIdAsync(chamado.ResponsavelId.Value);

                    if (responsavelAnterior != null && chamado.Status != ChamadoStatus.Fechado)
                    {
                        responsavelAnterior.ChamadosEmAberto--;
                        _unitOfWork.Responsaveis.Atualizar(responsavelAnterior);
                    }
                }

                // Encontrar responsável com menor carga
                var responsavelMenorCarga = await _unitOfWork.Responsaveis.ObterResponsavelComMenorCargaAsync();

                if (responsavelMenorCarga == null)
                {
                    throw new InvalidOperationException("Nenhum responsável disponível para atribuição");
                }

                // Atribuir chamado
                chamado.ResponsavelId = responsavelMenorCarga.Id;
                responsavelMenorCarga.ChamadosEmAberto++;

                _unitOfWork.Chamados.Atualizar(chamado);
                _unitOfWork.Responsaveis.Atualizar(responsavelMenorCarga);
                await _unitOfWork.Chamados.SalvarAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
