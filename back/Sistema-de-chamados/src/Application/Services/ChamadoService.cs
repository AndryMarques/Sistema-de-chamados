using AutoMapper;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;
using Sistema_de_chamados.src.Domain.Enums;

namespace Sistema_de_chamados.src.Application.Services
{
    public class ChamadoService : IChamadoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChamadoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ChamadoResponseDTO> CriarAsync(ChamadoCriarDTO chamadoCriarDTO)
        {
            try
            {
                // Verificar se usuário existe
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(chamadoCriarDTO.UsuarioId);

                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário com ID {chamadoCriarDTO.UsuarioId} não encontrado");
                }

                // Mapear DTO para entidade
                var chamado = _mapper.Map<Chamado>(chamadoCriarDTO);
                chamado.Status = ChamadoStatus.Aberto;
                chamado.DataAbertura = DateTime.UtcNow;

                // Atribuir automaticamente ao responsável com menor carga
                var responsavelMenorCarga = await _unitOfWork.Responsaveis.ObterResponsavelComMenorCargaAsync();

                if (responsavelMenorCarga != null)
                {
                    chamado.ResponsavelId = responsavelMenorCarga.Id;
                    responsavelMenorCarga.ChamadosEmAberto++;
                    _unitOfWork.Responsaveis.Atualizar(responsavelMenorCarga);
                }

                // Adicionar ao banco
                await _unitOfWork.Chamados.AdicionarAsync(chamado);
                await _unitOfWork.Chamados.SalvarAsync();

                return _mapper.Map<ChamadoResponseDTO>(chamado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ChamadoResponseDTO?> ObterPorIdAsync(int id)
        {
            try
            {
                var chamado = await _unitOfWork.Chamados.ObterComAcompanhamentosAsync(id);

                if (chamado == null)
                {
                    return null;
                }

                return _mapper.Map<ChamadoResponseDTO>(chamado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<ChamadoResponseDTO>> ObterTodosAsync()
        {
            try
            {
                var chamados = await _unitOfWork.Chamados.ObterTodosAsync();
                return _mapper.Map<IEnumerable<ChamadoResponseDTO>>(chamados);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ChamadoResponseDTO> AtualizarAsync(ChamadoAtualizarDTO chamadoDTO)
        {
            try
            {
                var chamado = await _unitOfWork.Chamados.ObterPorIdAsync(chamadoDTO.Id);

                if (chamado == null)
                {
                    throw new InvalidOperationException($"Chamado com ID {chamadoDTO.Id} não encontrado");
                }

                // Atualizar status
                var statusAnterior = chamado.Status;
                chamado.Status = chamadoDTO.Status;
                chamado.Prioridade = chamadoDTO.Prioridade;
                chamado.Titulo = chamadoDTO.Titulo;
                chamado.Descricao = chamadoDTO.Descricao;
                chamado.DataAtualizacao = DateTime.UtcNow;

                // Se mudar responsável
                if (chamado.ResponsavelId != chamadoDTO.ResponsavelId && chamadoDTO.ResponsavelId.HasValue)
                {
                    var responsavelAnterior = await _unitOfWork.Responsaveis.ObterPorIdAsync(chamado.ResponsavelId ?? 0);

                    if (responsavelAnterior != null && chamado.Status != ChamadoStatus.Fechado)
                    {
                        responsavelAnterior.ChamadosEmAberto--;
                        _unitOfWork.Responsaveis.Atualizar(responsavelAnterior);
                    }

                    var novoResponsavel = await _unitOfWork.Responsaveis.ObterPorIdAsync(chamadoDTO.ResponsavelId.Value);

                    if (novoResponsavel != null && chamado.Status != ChamadoStatus.Fechado)
                    {
                        novoResponsavel.ChamadosEmAberto++;
                        _unitOfWork.Responsaveis.Atualizar(novoResponsavel);
                    }

                    chamado.ResponsavelId = chamadoDTO.ResponsavelId;
                }

                // Se resolver o chamado
                if (chamadoDTO.Status == ChamadoStatus.Resolvido && statusAnterior != ChamadoStatus.Resolvido)
                {
                    chamado.DataResolucao = DateTime.UtcNow;
                }

                // Se fechar o chamado
                if (chamadoDTO.Status == ChamadoStatus.Fechado && statusAnterior != ChamadoStatus.Fechado)
                {
                    chamado.DataEncerramento = DateTime.UtcNow;

                    if (chamado.ResponsavelId.HasValue)
                    {
                        var responsavel = await _unitOfWork.Responsaveis.ObterPorIdAsync(chamado.ResponsavelId.Value);

                        if (responsavel != null)
                        {
                            responsavel.ChamadosEmAberto--;
                            _unitOfWork.Responsaveis.Atualizar(responsavel);
                        }
                    }
                }

                _unitOfWork.Chamados.Atualizar(chamado);
                await _unitOfWork.Chamados.SalvarAsync();

                return _mapper.Map<ChamadoResponseDTO>(chamado);
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
                var chamado = await _unitOfWork.Chamados.ObterPorIdAsync(id);

                if (chamado == null)
                {
                    throw new InvalidOperationException($"Chamado com ID {id} não encontrado");
                }

                // Não permitir deletar chamados que não estão fechados
                if (chamado.Status != ChamadoStatus.Fechado)
                {
                    throw new InvalidOperationException("Apenas chamados fechados podem ser deletados");
                }

                // Deletar acompanhamentos relacionados
                var acompanhamentos = await _unitOfWork.Acompanhamentos.ObterPorChamadoAsync(id);

                foreach (var acompanhamento in acompanhamentos)
                {
                    _unitOfWork.Acompanhamentos.Remover(acompanhamento);
                }

                _unitOfWork.Chamados.Remover(chamado);
                await _unitOfWork.Chamados.SalvarAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<ChamadoResponseDTO>> ObterPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var chamados = await _unitOfWork.Chamados.ObterPorUsuarioAsync(usuarioId);
                return _mapper.Map<IEnumerable<ChamadoResponseDTO>>(chamados);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<ChamadoResponseDTO>> ObterPorResponsavelAsync(int responsavelId)
        {
            try
            {
                var chamados = await _unitOfWork.Chamados.ObterPorResponsavelAsync(responsavelId);
                return _mapper.Map<IEnumerable<ChamadoResponseDTO>>(chamados);
            }
            catch
            {
                throw;
            }
        }
    }
}
