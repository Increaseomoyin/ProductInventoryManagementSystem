using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryManagementSystem.Data;
using ProductInventoryManagementSystem.DTOS.AccountDto;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController :ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _dataContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger _log;

        public AccountController(UserManager<AppUser> userManager, DataContext dataContext, SignInManager<AppUser> signInManager, ITokenService tokenService, ILogger<AccountController> log)
        {
           _userManager = userManager;
           _dataContext = dataContext;
           _signInManager = signInManager;
           _tokenService = tokenService;
            _log = log;
        }

        /// <summary>
        /// Login Endpoint
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromQuery] LoginDto loginUser)
        {
            _log.LogInformation($"{loginUser.UserName} is about to Log in! at {DateTime.Now}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var appUser = await _userManager.FindByNameAsync(loginUser.UserName);
            if (appUser == null)
            {
                ModelState.AddModelError("", "User does not exist");
                return StatusCode(404, ModelState);
            }
            else
            {
                var signedIn = await _signInManager.CheckPasswordSignInAsync(appUser, loginUser.Password, false);
                if (!signedIn.Succeeded)
                {
                    _log.LogWarning($"{loginUser.UserName} Login Failed at {DateTime.Now}");

                    ModelState.AddModelError("", "Something Happened");
                    return StatusCode(500, ModelState);
                }
                else
                {
                    var toks = await _tokenService.CreateToken(appUser);
                    var displayDto = new DisplayDto()
                    {
                        UserName = appUser.UserName,
                        AppUserId = appUser.Id,
                        Email = appUser.Email,
                        Token = toks

                    };
                    _log.LogInformation($"{loginUser.UserName} Logged in Successfully at {DateTime.Now}");

                    return Ok(displayDto);

                }

            }

        }
        /// <summary>
        /// Register Endpoint for Workers
        /// </summary>
        /// <param name="registerWorker"></param>
        /// <returns></returns>
        //Register for Worker
        [HttpPost("Register-Worker")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
       
        public async Task<IActionResult> RegisterWorker([FromQuery] RegisterDto registerWorker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _userManager.FindByNameAsync(registerWorker.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "The user already Exist!");
                return StatusCode(400, ModelState);
            }
            else
            {
                var appUser = new AppUser()
                {
                    UserName = registerWorker.Username,
                    Email = registerWorker.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerWorker.Password);
                if (createdUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Worker");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong!");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok($"{registerWorker.Username} Registered Successfully!");
        }

        /// <summary>
        /// Register Endpoint for Managers
        /// </summary>
        /// <param name="registerManager"></param>
        /// <returns></returns>
        //Register for Manager
        [HttpPost("Register-Manager")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
       
        public async Task<IActionResult> RegisterManager([FromQuery] RegisterDto registerManager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _userManager.FindByNameAsync(registerManager.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "The user already Exist!");
                return StatusCode(400, ModelState);
            }
            else
            {
                var appUser = new AppUser()
                {
                    UserName = registerManager.Username,
                    Email = registerManager.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerManager.Password);
                if (createdUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Manager");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong!");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok($"{registerManager.Username} Registered Successfully!");
        }

        /// <summary>
        /// Register Endpoint for Admins
        /// </summary>
        /// <param name="registerAdmin"></param>
        /// <returns></returns>
        //Register for Worker
        [HttpPost("Register-Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
      
        public async Task<IActionResult> RegisterAdmin([FromQuery] RegisterDto registerAdmin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _userManager.FindByNameAsync(registerAdmin.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "The user already Exist!");
                return StatusCode(400, ModelState);
            }
            else
            {
                var appUser = new AppUser()
                {
                    UserName = registerAdmin.Username,
                    Email = registerAdmin.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerAdmin.Password);
                if (createdUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong!");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok($"{registerAdmin.Username} Registered Successfully!");
        }

        
    }
}
