using AutoMapper;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.DTOs.PaymentMethods;
using GameStore.BLL.Services.ExternalServices.Responses;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class PaymentMethodProfile : Profile
{
    public PaymentMethodProfile()
    {
        CreateMap<PaymentMethod, PaymentMethodDto>();
        CreateMap<BoxPaymentResponse, BoxPaymentDto>()
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Sum, opt => opt.MapFrom(src => src.Amount));
    }
}