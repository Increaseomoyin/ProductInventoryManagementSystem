using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.DTOS.Product_Dto;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;
using ProductInventoryManagementSystem.Repositories;
using System.Text.Json;

namespace ProductInventoryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CategoryController :ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository,IDistributedCache distributedCache, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
           _cache = distributedCache;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Categories
        /// </summary>
        /// <returns></returns>
        //GET REQUESTS
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(ICollection<Category>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategories()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = "All-Categories";
            List<GetCategoryDto> categoriesMap;
            var categoriesFromCache = await _cache.GetStringAsync(cacheKey);
            if(categoriesFromCache !=null)
            {
                categoriesMap = JsonSerializer.Deserialize<List<GetCategoryDto>>(categoriesFromCache);
            }
            else
            {   
                //Get from database
                var categories = await _categoryRepository.GetCategories();
                categoriesMap = _mapper.Map<List<GetCategoryDto>>(categories);
                var serializedCategories = JsonSerializer.Serialize(categoriesMap);

                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(cacheKey, serializedCategories, cacheOptions);
            }

            return Ok(categoriesMap);
        }
        /// <summary>
        /// Get Category By Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("Id/{categoryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = $"Category-{categoryId}";
            GetCategoryDto categoryMap;
            var categoryFromCache = await _cache.GetStringAsync(cacheKey);
            if (categoryFromCache != null)
            {
                categoryMap = JsonSerializer.Deserialize<GetCategoryDto>(categoryFromCache);
            }
            else
            {
                var categoryExists = await _categoryRepository.CategoryExists(categoryId);
                if (!categoryExists)
                    return NotFound();
                var category = await _categoryRepository.GetCategoryById(categoryId);
                categoryMap = _mapper.Map<GetCategoryDto>(category);

                var serializedCategory = JsonSerializer.Serialize(categoryMap);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync(cacheKey, serializedCategory, cacheOptions);



            }

            return Ok(categoryMap);
        }

        //CREATE REQUESTS
        /// <summary>
        /// Create a new Category
        /// </summary>
        /// <param name="categoryCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]

        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryCreate)
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }
            var categoryExist = await _categoryRepository.GetCategoryByTitle(categoryCreate.Title);
            if (categoryExist != null)
            {
                ModelState.AddModelError("", "Category alredy Exist!");
                return StatusCode(400, ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryCreate);
            var category = await _categoryRepository.CreateCategory(categoryMap);
            if (!category)
            {
                ModelState.AddModelError("", "Something Happened!");
                return StatusCode(500, ModelState);
            }
            return Created();
        }
        //UPDATE REQUEST
        /// <summary>
        /// Update a Category with it's Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="categoryUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]


        public async Task<IActionResult> UpdateCategory([FromQuery] int categoryId, [FromBody] UpdateCategoryDto categoryUpdate)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            if (categoryId !=categoryUpdate.Id)
                return BadRequest(ModelState);
            if (! await _categoryRepository.CategoryExists(categoryUpdate.Id))
            {
                ModelState.AddModelError("", "Category does not exist");
                return StatusCode(404, ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryUpdate);
            var category = await _categoryRepository.UpdateCategory(categoryMap);
            if (!category)
            {
                ModelState.AddModelError("", "Something bad Happend!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Category-{categoryId}";
            await _cache.RemoveAsync(cacheKey);     
            return NoContent();
        }
        //DELETE REQUEST
        /// <summary>
        /// Delete a Category with it's Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="categoryDelete"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Manger")]


        public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId, DeleteCategoryDto categoryDelete)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (categoryId != categoryDelete.Id)
                return BadRequest(ModelState);
            if (!await _categoryRepository.CategoryExists(categoryDelete.Id))
            {
                ModelState.AddModelError("", "Category does not exist");
                return StatusCode(404, ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryDelete);
            var category = await _categoryRepository.DeleteCategory(categoryMap);
            if (!category)
            {
                ModelState.AddModelError("", "Something bad Happend!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Category-{categoryId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();

        }

    }
}
