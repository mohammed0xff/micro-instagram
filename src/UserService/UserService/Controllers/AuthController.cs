using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Api.Requests;
using UserService.Api.Responses;
using UserService.Common;
using UserService.DBContext;
using UserService.Entities;
using BC = BCrypt.Net.BCrypt;


namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly TokenConfiguration _tokenConfiguration;

        public AuthController(
            AppDbContext dbContext, 
            IOptions<TokenConfiguration> tokenConfiguration
            )
        {
            _tokenConfiguration = tokenConfiguration.Value;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            User created = new User()
            {
                Username = request.Username,
                Email = request.Email,
            };

            _dbContext.Users.Add(created);
            created.Password = BC.HashPassword(request.Password);

            await _dbContext.SaveChangesAsync();
            
            // todo :: map created to UserResponse
            return CreatedAtAction(nameof(Register), new UserResponse() { Username = created.Username}); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username.ToLower());
            
            if (user == null || !BC.Verify(request.Password, user.Password))
            {
                return Unauthorized("Username or password is incorrect");
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenConfiguration.Secret);
            var claims = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new("username", user.Username)
            });

            var expDate = DateTime.UtcNow
                .AddMinutes(_tokenConfiguration.DurationInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = expDate,
                Audience = _tokenConfiguration.Audience,
                Issuer = _tokenConfiguration.Issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                    ),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new JwtResponse
            {
                Token = tokenHandler.WriteToken(token),
                ExpDate = expDate
            });
        }
    }
}
