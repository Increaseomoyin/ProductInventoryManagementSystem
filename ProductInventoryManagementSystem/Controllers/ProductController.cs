using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.DTOS.Product_Dto;
using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;
using ProductInventoryManagementSystem.Repositories;
using System;
using System.Text.Json;

namespace ProductInventoryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ProductController :ControllerBase
    {  

        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public ProductController(IProductRepository productRepository, IMapper mapper, IDistributedCache cache)
        {
            _productRepository = productRepository;
           _mapper = mapper;
            _cache = cache;
        }

        //GET REQUESTS
        /// <summary>
        /// Get all Products
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(ICollection<Product>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProducts([FromQuery] QueryableProduct query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
             var products = await _productRepository.GetProducts(query);
            var productsMap = _mapper.Map<List<GetProductDto>>(products);

            return Ok(productsMap);
         }
        /// <summary>
        /// Get Product By Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("Id/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = $"Product-{productId}";
            GetProductDto productMap;
            var productFromCache = await _cache.GetStringAsync(cacheKey);
            if (productFromCache != null)
            {
                productMap = JsonSerializer.Deserialize<GetProductDto>(productFromCache);
            }
            else
            {

                var productExists = await _productRepository.ProductExists(productId);
                if (!productExists)
                    return NotFound();
                var product = await _productRepository.GetProductById(productId);
                productMap = _mapper.Map<GetProductDto>(product);

                var serializedProduct = JsonSerializer.Serialize(productMap);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, serializedProduct, cacheOptions);
            }

           
            return Ok(productMap);
        }

        /// <summary>
        /// Get Product By Name
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        [HttpGet("Name/{productName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductByName([FromRoute] string productName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = $"Product-{productName}";
            GetProductDto productMap;
            var productFromCache = await _cache.GetStringAsync(cacheKey);
            if (productFromCache != null)
            {
                productMap = JsonSerializer.Deserialize<GetProductDto>(productFromCache);
            }
            else
            {
                var product = await _productRepository.GetProductByName(productName);
                if (product == null)
                {
                    return NotFound();
                }
                productMap = _mapper.Map<GetProductDto>(product);

                var serializedProduct = JsonSerializer.Serialize(productMap);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(cacheKey, serializedProduct, cacheOptions);
            }
            
            return Ok(productMap);
        }
        //CREATE REQUEST
        /// <summary>
        /// Create a new Product
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="productCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]


        public async Task<IActionResult> CreateProduct([FromQuery] List<int> categoryIds, [FromBody] CreateProductDto productCreate)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            var productExist = await _productRepository.GetProductByName(productCreate.Name);
            if (productExist != null)
            {
                ModelState.AddModelError("", "Product Already Exist!");
                return StatusCode(400, ModelState);
            }
            var productMap = _mapper.Map<Product>(productCreate);
            var product = await _productRepository.CreateProduct(categoryIds, productMap);
            if (!product)
            {
                ModelState.AddModelError("", "Something Bad Happened!");
                return StatusCode(500, ModelState);
            }
            return Created();

        }
        //UPDATE REQUEST
        /// <summary>
        /// Update a Product by it's Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="CategoryIds"></param>
        /// <param name="productUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]


        public async Task<IActionResult> UpdateProduct([FromQuery] int productId, [FromQuery] List<int> CategoryIds, [FromBody] UpdateProductDto productUpdate)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (productId != productUpdate.Id)
                return BadRequest(ModelState);
            if (!await _productRepository.ProductExists(productId))
                return NotFound();
            var productMap = _mapper.Map<Product>(productUpdate);
            var product = await _productRepository.UpdateProduct(CategoryIds, productMap);
            if (!product)
            {
                ModelState.AddModelError("", "Something Bad Happened!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Product-{productId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();
        }
        //DELETE REQUEST
        /// <summary>
        /// Delete a Product by it's Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productDelete"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]


        public async Task<IActionResult> DeleteProduct([FromQuery] int productId, DeleteProductDto productDelete)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (productId != productDelete.Id)
                return BadRequest(ModelState);
            if (!await _productRepository.ProductExists(productDelete.Id))
            {
                ModelState.AddModelError("", "Category does not exist");
                return StatusCode(404, ModelState);
            }
            var productMap = _mapper.Map<Product>(productDelete);
            var product = await _productRepository.DeleteProduct(productMap);
            if (!product)
            {
                ModelState.AddModelError("", "Something bad Happend!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Product-{productId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();

        }
    }
}
