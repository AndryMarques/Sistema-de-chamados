using AutoMapper;
using Sistema_de_chamados.src.API.DTOs;
using Sistema_de_chamados.src.Domain.Entities;

namespace Sistema_de_chamados.src.Infrastructure.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Usuario Mappings
            CreateMap<Usuario, UsuarioResponseDTO>().ReverseMap();
            CreateMap<UsuarioCriarDTO, Usuario>();
            CreateMap<UsuarioAtualizarDTO, Usuario>();

            // Chamado Mappings
            CreateMap<Chamado, ChamadoResponseDTO>()
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
                .ForMember(dest => dest.Responsavel, opt => opt.MapFrom(src => src.Responsavel))
                .ForMember(dest => dest.Acompanhamentos, opt => opt.MapFrom(src => src.Acompanhamentos))
                .ReverseMap();
            CreateMap<ChamadoCriarDTO, Chamado>();
            CreateMap<ChamadoAtualizarDTO, Chamado>();

            // Responsavel Mappings
            CreateMap<Responsavel, ResponsavelResponseDTO>()
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
                .ReverseMap();
            CreateMap<ResponsavelCriarDTO, Responsavel>();

            // Acompanhamento Mappings
            CreateMap<Acompanhamento, AcompanhamentoResponseDTO>()
                .ForMember(dest => dest.Responsavel, opt => opt.MapFrom(src => src.Responsavel))
                .ReverseMap();
            CreateMap<AcompanhamentoCriarDTO, Acompanhamento>();
        }
    }
}
