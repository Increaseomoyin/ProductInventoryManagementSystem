using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductInventoryManagementSystem.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public async Task<string> CreateToken(AppUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email ),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName ),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),

            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return  tokenHandler.WriteToken(token);


            
                

               
        }
    }
}
