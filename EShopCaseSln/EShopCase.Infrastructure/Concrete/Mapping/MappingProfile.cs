using AutoMapper;
using EShopCase.Application.Dtos.CategoryDtos;
using EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;
using EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;
using EShopCase.Domain.Entities;

namespace EShopCase.Infrastructure.Concrete.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
       CreateMap<Category,CategoryResponseDto>().ReverseMap();
       CreateMap<Products,CreateProductCommandRequest>().ReverseMap();

       CreateMap<Products, GetAllProductQueryResponse>()
           .ForMember(d => d.CategoryResponse, 
               o => o.MapFrom(s => s.Category));
    }
}