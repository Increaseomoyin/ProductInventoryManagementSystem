using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductInventoryManagementSystem.DTOS;
using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.DTOS.Sale_Dto;
using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;
using ProductInventoryManagementSystem.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductInventoryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public SaleController(ISaleRepository saleRepository, IMapper mapper, IDistributedCache cache)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _cache = cache;
        }

        //GET REQUESTS
        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(ICollection<Sale>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSales([FromQuery] QueryableSale query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sales = await _saleRepository.GetSales(query);
            var salesMap = _mapper.Map<List<GetSaleDto>>(sales);
              return Ok(salesMap);


         }
    //CREATE REQUESTS
    /// <summary>
    /// Create a new Sale
    /// </summary>
    /// <param name="saleCreate"></param>
    /// <returns></returns>
    [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Worker")]


        public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto saleCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var saleMap = _mapper.Map<Sale>(saleCreate);
            var sale = await _saleRepository.CreateSale(saleMap);
            return NoContent();
        }

        //UPDATE REQUEST
        /// <summary>
        /// Update a sale by it's Id
        /// </summary>
        /// <param name="SaleId"></param>
        /// <param name="saleUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Worker")]

        public async Task<IActionResult> UpdateSale([FromQuery] int SaleId, [FromBody] UpdateSaleDto saleUpdate)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (SaleId != saleUpdate.Id)
                return BadRequest(ModelState);
            var saleMap = _mapper.Map<Sale>(saleUpdate);
            var sale = await _saleRepository.UpdateSale(saleMap);
            if (!sale)
            {
                ModelState.AddModelError("", "Something Bad Happened!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Sale-{SaleId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();
        }
        //DELETE REQUEST
        /// <summary>
        /// Delete a Sale by it's Id
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="saleDelete"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin, Worker")]

        public async Task<IActionResult> DeleteSale([FromQuery] int saleId, DeleteCategoryDto saleDelete)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (saleId != saleDelete.Id)
                return BadRequest(ModelState);
           
            var saleMap = _mapper.Map<Sale>(saleDelete);
            var sale = await _saleRepository.DeleteSale(saleMap);
            if (!sale)
            {
                ModelState.AddModelError("", "Something bad Happend!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"Sale-{saleId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();

        }
    }
}
