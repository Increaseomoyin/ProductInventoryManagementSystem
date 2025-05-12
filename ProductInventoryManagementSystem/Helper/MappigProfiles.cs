using AutoMapper;
using ProductInventoryManagementSystem.DTOS;
using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.DTOS.Product_Dto;
using ProductInventoryManagementSystem.DTOS.Sale_Dto;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Helper
{
    public class MappigProfiles : Profile
    {
        public MappigProfiles()
        {
            CreateMap<ProfileUser, GetUserDto>();
            CreateMap<GetUserDto,  ProfileUser> ();
            CreateMap<ProfileUser, CreateUserDto>();
            CreateMap<CreateUserDto, ProfileUser>();
            CreateMap<ProfileUser, UpdateUserDto>();
            CreateMap<UpdateUserDto, ProfileUser>();
            CreateMap<ProfileUser, DeleteUserDto>();
            CreateMap<DeleteUserDto, ProfileUser>();

            CreateMap<Product, GetProductDto>();
            CreateMap<GetProductDto, Product> ();
            CreateMap<Product, CreateProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, UpdateProductDto>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, DeleteProductDto>();
            CreateMap<DeleteProductDto, Product>();

            CreateMap<Category, GetCategoryDto>();
            CreateMap<GetCategoryDto, Category>();
            CreateMap<Category, CreateCategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Category, UpdateCategoryDto>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, DeleteCategoryDto>();
            CreateMap<DeleteCategoryDto, Category>();


            CreateMap<Sale, GetSaleDto>();
            CreateMap<GetSaleDto, Sale>();
            CreateMap<Sale, CreateSaleDto>();
            CreateMap<CreateSaleDto, Sale>();
            CreateMap<Sale, UpdateSaleDto>();
            CreateMap<UpdateSaleDto, Sale>();
            CreateMap<DeleteSaleDto, Sale>();
            CreateMap<Sale, DeleteSaleDto>();

        }
    }
}
