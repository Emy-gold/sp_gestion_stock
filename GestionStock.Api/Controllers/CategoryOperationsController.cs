using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryOperationsController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public CategoryOperationsController(GestionStockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryOperationDto>>> GetAll()
    {
        var categories = await _context.CategoryOperations
            .Select(c => new CategoryOperationDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                Attributes = c.Attributes
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryOperationDto>> GetById(int id)
    {
        var category = await _context.CategoryOperations.FindAsync(id);
        if (category is null)
            return NotFound();

        return Ok(new CategoryOperationDto
        {
            Id = category.Id,
            Nom = category.Nom,
            Description = category.Description,
            Attributes = category.Attributes
        });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryOperationDto>> Create(CategoryOperationCreateDto dto)
    {
        var category = new CategoryOperation
        {
            Nom = dto.Nom,
            Description = dto.Description,
            Attributes = dto.Attributes
        };

        _context.CategoryOperations.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryOperationCreateDto dto)
    {
        var category = await _context.CategoryOperations.FindAsync(id);
        if (category is null)
            return NotFound();

        category.Nom = dto.Nom;
        category.Description = dto.Description;
        category.Attributes = dto.Attributes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hasOperations = await _context.Operations.AnyAsync(o => o.CategoryOperationId == id);
        if (hasOperations)
            return BadRequest("Impossible de supprimer : des opérations utilisent cette catégorie.");

        var category = await _context.CategoryOperations.FindAsync(id);
        if (category is null)
            return NotFound();

        _context.CategoryOperations.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}