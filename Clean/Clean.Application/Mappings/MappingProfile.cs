using AutoMapper;
using Clean.Application.Features.Products.Commands;
using Clean.Domain.Entities;

namespace Clean.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()

        {
            // source, destination
            CreateMap<CreateProductCommand, Product>();

            //update mapping
            CreateMap<UpdateProductCommand, Product>();

            //if names are different, you can specify the mapping like this:
            //CreateMap<CreateProductCommand, Product>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
            //    .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.ProductRate));

            // if you want to ignore certain properties during mapping, you can do it like this:    
            //CreateMap<CreateProductCommand, Product>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore());

            //reverse mapping src destination bn jata hy destination src
    //        CreateMap<Product, Product>()
    //.ReverseMap();



        }
    }
}
