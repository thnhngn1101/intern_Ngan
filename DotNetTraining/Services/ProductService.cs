using System.Data;
using Application.Settings;
using AutoMapper;
using Common.Application.CustomAttributes;
using Common.Application.Exceptions;
using Common.Services;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Entities;
using DotNetTraining.Repositories;

namespace DotNetTraining.Services
{
    [ScopedService]
    public class ProductService(IServiceProvider services, ApplicationSetting setting, IDbConnection connection) : BaseService(services)
    {

        private readonly ProductRepository _repo = new(connection);

        public async Task<List<ProductDto>> GetAllProduct()
        {
            var products = await _repo.GetAllProduct();
            var result = _mapper.Map<List<ProductDto>>(products);
            return result;

        }

        public async Task<ProductDto?> GetProductById(Guid productId)
        {
            var existingProduct = await _repo.GetProductById(productId);

            if(existingProduct == null)
            {
                throw new Exception("Product not exist");
            }
            // map entity to Dto
            var dto = _mapper.Map<ProductDto>(existingProduct);
            
            return dto;
        }

        public async Task<Product?> UpdateProduct(Guid productId,ProductDto productDto)
        {
            var existingProduct = await _repo.GetProductById(productId);
            if (existingProduct == null)
            {
                throw new Exception(" id not found");
            }

            // Cập nhật thông tin từ DTO
            var product = _mapper.Map(productDto, existingProduct);

            var updatedProduct = await _repo.UpdateProduct(product);

            return product;
        }

        public async Task DeleteProduct(Guid productId)
        {
            var existingProduct = await _repo.GetProductById(productId);
            if (existingProduct == null)
            {
                throw new Exception("Product not exist");
            }

            await _repo.DeleteProduct(existingProduct);
        }

        public async Task<Product?> CreateProduct(ProductDto newProduct)
        {
            var product = _mapper.Map<Product>(newProduct);
            product.Id = Guid.NewGuid();

            // Gọi repository để lưu vào DB
            return await _repo.CreateProduct(product);
        }
    }
}

