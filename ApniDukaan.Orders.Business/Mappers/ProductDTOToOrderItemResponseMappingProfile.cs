using ApniDukaan.Orders.Business.ResponseDTO;
using ApniDukaan.Orders.Data.Entities;
using AutoMapper;

namespace ApniDukaan.Orders.Business.Mappers
{
    public class ProductDTOToOrderItemResponseMappingProfile : Profile
    {
        public ProductDTOToOrderItemResponseMappingProfile()
        {
            CreateMap<ProductDTO, OrderItemResponse>()
              .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
              .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
        }
    }
}
