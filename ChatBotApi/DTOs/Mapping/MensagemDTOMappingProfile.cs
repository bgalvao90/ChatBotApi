using AutoMapper;
using ChatBotApi.Models;

namespace ChatBotApi.DTOs.Mapping
{
    public class MensagemDTOMappingProfile : Profile
    {
        public MensagemDTOMappingProfile()
        {
            CreateMap<MensagemEntradaDto, Mensagem>();
            CreateMap<RespostaAtendenteDto, Mensagem>();
            CreateMap<Mensagem, MensagemDto>();
            CreateMap<Atendimento, AtendimentoDto>();

        }
    }
}
