using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly GestionStockDbContext _context;

        public RolesController(GestionStockDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Nom = r.Nom,
                Description = r.Description
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return new RoleDto
            {
                Id = role.Id,
                Nom = role.Nom,
                Description = role.Description
            };
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(RoleCreateDto dto)
        {
            var role = new Role
            {
                Nom = dto.Nom,
                Description = dto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Nom = role.Nom,
                Description = role.Description
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, roleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleCreateDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            role.Nom = dto.Nom;
            role.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
