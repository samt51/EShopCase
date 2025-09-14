using AutoMapper;
using EShopCase.Application.Dtos.CategoryDtos;
using EShopCase.Application.Dtos.UserDtos;
using EShopCase.Application.Features.CategoryFeature.Commands.CreateCategory;
using EShopCase.Application.Features.CategoryFeature.Queries.GetByIdCategory;
using EShopCase.Application.Features.OrderFeature.Commands.CreateOrder;
using EShopCase.Application.Features.OrderFeature.Queries.GetByOrder;
using EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;
using EShopCase.Application.Features.ProductsFeature.Commands.UpdateProduct;
using EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;
using EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;
using EShopCase.Application.Features.UserFeature.Commands.Register;
using EShopCase.Domain.Entities;

namespace EShopCase.Infrastructure.Concrete.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
       CreateMap<Category,CategoryResponseDto>().ReverseMap();
       CreateMap<Category,CreateCategoryCommandRequest>().ReverseMap();
       CreateMap<Category,GetByIdCategoryQueryResponse>().ReverseMap();

       CreateMap<Users,RegisterCommandRequest>().ReverseMap();

       CreateMap<Products,CreateProductCommandRequest>().ReverseMap();
       CreateMap<Products,UpdateProductCommandRequest>().ReverseMap();

       CreateMap<Users, UserResponseDto>().ReverseMap();

       CreateMap<Products, GetByIdProductQueryResponse>()
           .ForMember(d => d.CategoryResponse, 
               o => o.MapFrom(s => s.Category));

       CreateMap<Order, CreateOrderCommandRequest>().ReverseMap();
       CreateMap<Order, GetByOrderQueryResponse>().ReverseMap()
           .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItemsRequest)).
           ForMember(d=>d.Users,o=>o.MapFrom(s=>s.UserResponseDto));




    }
}