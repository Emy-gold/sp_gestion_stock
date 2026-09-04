using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public UsersController(GestionStockDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _context.ApplicationUsers
            .Include(u => u.Role)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Nom = u.Nom,
                Prenom = u.Prenom,
                Email = u.Email,
                Telephone = u.Telephone,
                RoleId = u.RoleId,
                RoleNom = u.Role != null ? u.Role.Nom : null
            })
            .OrderBy(u => u.Nom)
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.ApplicationUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Nom = user.Nom,
            Prenom = user.Prenom,
            Email = user.Email,
            Telephone = user.Telephone,
            RoleId = user.RoleId,
            RoleNom = user.Role?.Nom
        });
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("L'email est requis.");

        var emailExists = await _context.ApplicationUsers
            .AnyAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower());

        if (emailExists)
            return BadRequest("Un utilisateur avec cet email existe déjà.");

        var user = new ApplicationUser
        {
            Nom = dto.Nom.Trim(),
            Prenom = dto.Prenom.Trim(),
            Email = dto.Email.Trim(),
            Telephone = dto.Telephone?.Trim(),
            MotDePasse = string.IsNullOrWhiteSpace(dto.MotDePasse) ? "123456" : dto.MotDePasse.Trim(),
            RoleId = dto.RoleId
        };

        _context.ApplicationUsers.Add(user);
        await _context.SaveChangesAsync();

        if (user.RoleId.HasValue)
        {
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
        }

        var result = new UserDto
        {
            Id = user.Id,
            Nom = user.Nom,
            Prenom = user.Prenom,
            Email = user.Email,
            Telephone = user.Telephone,
            RoleId = user.RoleId,
            RoleNom = user.Role?.Nom
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, result);
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateDto dto)
    {
        var user = await _context.ApplicationUsers.FindAsync(id);
        if (user == null)
            return NotFound();

        var emailExists = await _context.ApplicationUsers
            .AnyAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower() && u.Id != id);

        if (emailExists)
            return BadRequest("Un autre utilisateur avec cet email existe déjà.");

        user.Nom = dto.Nom.Trim();
        user.Prenom = dto.Prenom.Trim();
        user.Email = dto.Email.Trim();
        user.Telephone = dto.Telephone?.Trim();
        if (!string.IsNullOrWhiteSpace(dto.MotDePasse))
        {
            user.MotDePasse = dto.MotDePasse.Trim();
        }
        user.RoleId = dto.RoleId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.ApplicationUsers.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.ApplicationUsers.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
