using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjetoLoginRegister.Api.Data;
using ProjetoLoginRegister.Domain;
using ProjetoLoginRegister.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjetoLoginRegister.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProjetoContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public AuthController
                       (ProjetoContext context, 
                       IConfiguration configuration, 
                       IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] UserRegister model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Usuario
            {
                Nome = model.Nome,
                Role = model.Role,
                Username = model.Username
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new {Message = "Usuário registrado com sucesso." });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Usuarios.Where(u => u.Username == userLogin.Username)
                .Select(u => new {u.Username, u.PasswordHash, u.Nome, u.Role}).FirstOrDefaultAsync();

            if(user == null ||  _passwordHasher.VerifyHashedPassword
                (null,user.PasswordHash,userLogin.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new {Message = "Credenciais inválidas"});
            }

            var token = GenerateJwtToken(user.Username, user.Nome, user.Role);

            return Ok(token);
        }
        private string GenerateJwtToken(string username, string nome, string role)
        {
            var claims = new[]
            {
        new Claim ("nome", "valor"),
        new Claim(ClaimTypes.Name, nome),
        new Claim(ClaimTypes.NameIdentifier, username),
        new Claim(ClaimTypes.Role, role) // Include role in the token
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
