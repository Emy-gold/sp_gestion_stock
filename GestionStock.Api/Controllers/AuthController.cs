using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly GestionStockDbContext _context;

    public AuthController(IConfiguration configuration, GestionStockDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest(new LoginResponseDto
            {
                IsSuccess = false,
                Message = "Email et mot de passe requis."
            });
        }

        // 1. Check Admin Account from appsettings.json
        var adminEmail = _configuration["AdminAccount:Email"] ?? "admin@standardprofil.com";
        var adminPassword = _configuration["AdminAccount:Password"] ?? "AdminPassword123!";
        var adminNom = _configuration["AdminAccount:Nom"] ?? "Admin";
        var adminPrenom = _configuration["AdminAccount:Prenom"] ?? "Standard Profil";

        if (string.Equals(loginDto.Email.Trim(), adminEmail.Trim(), StringComparison.OrdinalIgnoreCase) &&
            loginDto.Password == adminPassword)
        {
            return Ok(new LoginResponseDto
            {
                IsSuccess = true,
                Email = adminEmail,
                Nom = adminNom,
                Prenom = adminPrenom,
                Role = "Administrateur",
                Token = Guid.NewGuid().ToString(),
                Message = "Connexion réussie en tant qu'administrateur."
            });
        }

        // 2. Check Database Users
        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.Trim().ToLower());

        if (user != null)
        {
            return Ok(new LoginResponseDto
            {
                IsSuccess = true,
                Email = user.Email,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Role = "Utilisateur",
                Token = Guid.NewGuid().ToString(),
                Message = "Connexion réussie."
            });
        }

        return Unauthorized(new LoginResponseDto
        {
            IsSuccess = false,
            Message = "Identifiants invalides. Vérifiez votre email et mot de passe."
        });
    }
}
