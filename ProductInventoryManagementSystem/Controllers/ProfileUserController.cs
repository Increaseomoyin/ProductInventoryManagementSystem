using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProductInventoryManagementSystem.DTOS;
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
    public class ProfileUserController : ControllerBase
    {
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly UserManager<AppUser> _userManager;

        public ProfileUserController(IProfileUserRepository profileUserRepository, IMapper mapper, IDistributedCache cache, UserManager<AppUser> userManager )
        {
            _profileUserRepository = profileUserRepository;
            _mapper = mapper;
            _cache = cache;
            _userManager = userManager;
        }
        //GET REQUESTS
        /// <summary>
        /// Get all ProfileUsers
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(ICollection<ProfileUser>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProfileUsers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = "All-Users";
            List<GetUserDto> profileUsersMap;
            var userFromCache = await _cache.GetStringAsync(cacheKey);
            if( userFromCache != null)
            {
                profileUsersMap = JsonSerializer.Deserialize<List<GetUserDto>>(userFromCache);
            }
            else
            {
                var profileUsers = await _profileUserRepository.GetProfileUsers();
                profileUsersMap = _mapper.Map<List<GetUserDto>>(profileUsers);

                var serializedUsers = JsonSerializer.Serialize(profileUsersMap);
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };    
                await _cache.SetStringAsync(cacheKey, serializedUsers, cacheOptions);
            }
           
            return Ok(profileUsersMap);
        }

        /// <summary>
        /// Get ProfileUser by Id
        /// </summary>
        /// <param name="profileUserId"></param>
        /// <returns></returns>
        [HttpGet("Id/{profileUserId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProfileUserById([FromRoute] int profileUserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cacheKey = "All-Users";
            GetUserDto profileUserMap;
            var userFromCache = await _cache.GetStringAsync(cacheKey);
            if (userFromCache != null)
            {
                profileUserMap = JsonSerializer.Deserialize<GetUserDto>(userFromCache);
            }
            else
            {
                if (!await _profileUserRepository.ProfileUserExists(profileUserId))
                    return NotFound();
                var profileUser = await _profileUserRepository.GetProfileUserById(profileUserId);
                profileUserMap = _mapper.Map<GetUserDto>(profileUser);

                var serializedUser = JsonSerializer.Serialize(profileUserMap);
                var cacheOPtions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, serializedUser, cacheOPtions);
            }
                
            return Ok(profileUserMap);
        }

        //CREATE REQUEST
        /// <summary>
        /// Create a new ProfileUser
        /// </summary>
        /// <param name="profileUserCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProfileUser([FromBody] CreateUserDto profileUserCreate)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            var validAppUserId = await _userManager.FindByIdAsync(profileUserCreate.AppUserId);
            if (validAppUserId == null)
                return NotFound(ModelState);

            var userMap = _mapper.Map<ProfileUser>(profileUserCreate);
            var user = await _profileUserRepository.CreateProfileUser(userMap);
            if(!user)
            {
                ModelState.AddModelError("", "Something Happened!");
                return StatusCode(500, ModelState);
            }   
            return NoContent();
        }
        //UPDATE REQUEST
        /// <summary>
        /// Update a ProfileUser by it's Id
        /// </summary>
        /// <param name="ProfileUserId"></param>
        /// <param name="profileUserUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> UpdateProfileUser( [FromQuery] int ProfileUserId, [FromBody] UpdateUserDto profileUserUpdate)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (ProfileUserId != profileUserUpdate.Id)
                return BadRequest(ModelState);
            if (!await _profileUserRepository.ProfileUserExists(profileUserUpdate.Id))
                return NotFound();
            var profileUserMap = _mapper.Map<ProfileUser>(profileUserUpdate);
            var profileUser = await _profileUserRepository.UpdateProfileUser(profileUserMap);
            if (!profileUser)
            {
                ModelState.AddModelError("", "Something Bad Happened!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"User-{ProfileUserId}";
            await _cache.RemoveAsync(cacheKey);

            return NoContent();
        }
        //DELETE REQUEST
        /// <summary>
        /// Delete a ProfileUser by it's Id
        /// </summary>
        /// <param name="profileUserId"></param>
        /// <param name="userDelete"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteProfileUser([FromQuery] int profileUserId, DeleteUserDto userDelete)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (profileUserId != userDelete.Id)
                return BadRequest(ModelState);
            if (!await _profileUserRepository.ProfileUserExists(userDelete.Id))
            {
                ModelState.AddModelError("", "Category does not exist");
                return StatusCode(404, ModelState);
            }
            var profileUserMap = _mapper.Map<ProfileUser>(userDelete);
            var profileUser = await _profileUserRepository.DeleteProfileUser(profileUserMap);
            if (!profileUser)
            {
                ModelState.AddModelError("", "Something bad Happend!");
                return StatusCode(500, ModelState);
            }
            var cacheKey = $"User-{profileUserId}";
            await _cache.RemoveAsync(cacheKey);
            return NoContent();

        }
    }
}
