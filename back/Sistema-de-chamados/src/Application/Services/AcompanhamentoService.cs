using AutoMapper;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Application.Interfaces;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Application.Services
{
    public class AcompanhamentoService : IAcompanhamentoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AcompanhamentoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AcompanhamentoResponseDTO> CriarAsync(AcompanhamentoCriarDTO acompanhamentoDTO)
        {
            try
            {
                // Verificar se chamado existe
                var chamado = await _unitOfWork.Chamados.ObterPorIdAsync(acompanhamentoDTO.ChamadoId);

                if (chamado == null)
                {
                    throw new InvalidOperationException($"Chamado com ID {acompanhamentoDTO.ChamadoId} não encontrado");
                }

                // Verificar se responsável existe
                var responsavel = await _unitOfWork.Responsaveis.ObterPorIdAsync(acompanhamentoDTO.ResponsavelId);

                if (responsavel == null)
                {
                    throw new InvalidOperationException($"Responsável com ID {acompanhamentoDTO.ResponsavelId} não encontrado");
                }

                // Mapear DTO para entidade
                var acompanhamento = _mapper.Map<Acompanhamento>(acompanhamentoDTO);
                acompanhamento.DataAcompanhamento = DateTime.UtcNow;

                // Adicionar ao banco
                await _unitOfWork.Acompanhamentos.AdicionarAsync(acompanhamento);
                await _unitOfWork.Acompanhamentos.SalvarAsync();

                return _mapper.Map<AcompanhamentoResponseDTO>(acompanhamento);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AcompanhamentoResponseDTO?> ObterPorIdAsync(int id)
        {
            try
            {
                var acompanhamento = await _unitOfWork.Acompanhamentos.ObterPorIdAsync(id);

                if (acompanhamento == null)
                {
                    return null;
                }

                return _mapper.Map<AcompanhamentoResponseDTO>(acompanhamento);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<AcompanhamentoResponseDTO>> ObterPorChamadoAsync(int chamadoId)
        {
            try
            {
                // Verificar se chamado existe
                var chamado = await _unitOfWork.Chamados.ObterPorIdAsync(chamadoId);

                if (chamado == null)
                {
                    throw new InvalidOperationException($"Chamado com ID {chamadoId} não encontrado");
                }

                var acompanhamentos = await _unitOfWork.Acompanhamentos.ObterPorChamadoAsync(chamadoId);
                return _mapper.Map<IEnumerable<AcompanhamentoResponseDTO>>(acompanhamentos);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<AcompanhamentoResponseDTO>> ObterPorResponsavelAsync(int responsavelId)
        {
            try
            {
                // Verificar se responsável existe
                var responsavel = await _unitOfWork.Responsaveis.ObterPorIdAsync(responsavelId);

                if (responsavel == null)
                {
                    throw new InvalidOperationException($"Responsável com ID {responsavelId} não encontrado");
                }

                var acompanhamentos = await _unitOfWork.Acompanhamentos.ObterPorResponsavelAsync(responsavelId);
                return _mapper.Map<IEnumerable<AcompanhamentoResponseDTO>>(acompanhamentos);
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
                var acompanhamento = await _unitOfWork.Acompanhamentos.ObterPorIdAsync(id);

                if (acompanhamento == null)
                {
                    throw new InvalidOperationException($"Acompanhamento com ID {id} não encontrado");
                }

                _unitOfWork.Acompanhamentos.Remover(acompanhamento);
                await _unitOfWork.Acompanhamentos.SalvarAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
