using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryArticlesController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public CategoryArticlesController(GestionStockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryArticleDto>>> GetAll()
    {
        var categories = await _context.CategoryArticles
            .Include(c => c.Parent)
            .Select(c => new CategoryArticleDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                Image = c.Image,
                Attributes = c.Attributes,
                ParentId = c.ParentId,
                ParentNom = c.Parent != null ? c.Parent.Nom : null
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryArticleDto>> GetById(int id)
    {
        var category = await _context.CategoryArticles
            .Include(c => c.Parent)
            .Where(c => c.Id == id)
            .Select(c => new CategoryArticleDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                Image = c.Image,
                Attributes = c.Attributes,
                ParentId = c.ParentId,
                ParentNom = c.Parent != null ? c.Parent.Nom : null
            })
            .FirstOrDefaultAsync();

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryArticleDto>> Create(CategoryArticleCreateDto dto)
    {
        if (dto.ParentId.HasValue)
        {
            var parentExists = await _context.CategoryArticles.AnyAsync(c => c.Id == dto.ParentId);
            if (!parentExists)
                return BadRequest("ParentId invalide.");
        }

        var category = new CategoryArticle
        {
            Nom = dto.Nom,
            Description = dto.Description,
            Image = dto.Image,
            Attributes = dto.Attributes,
            ParentId = dto.ParentId
        };

        _context.CategoryArticles.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryArticleCreateDto dto)
    {
        var category = await _context.CategoryArticles.FindAsync(id);
        if (category is null)
            return NotFound();

        if (dto.ParentId == id)
            return BadRequest("Une catégorie ne peut pas être son propre parent.");

        category.Nom = dto.Nom;
        category.Description = dto.Description;
        category.Image = dto.Image;
        category.Attributes = dto.Attributes;
        category.ParentId = dto.ParentId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hasArticles = await _context.Articles.AnyAsync(a => a.CategoryArticleId == id);
        if (hasArticles)
            return BadRequest("Impossible de supprimer : des articles utilisent cette catégorie.");

        var hasChildren = await _context.CategoryArticles.AnyAsync(c => c.ParentId == id);
        if (hasChildren)
            return BadRequest("Impossible de supprimer : cette catégorie a des sous-catégories.");

        var category = await _context.CategoryArticles.FindAsync(id);
        if (category is null)
            return NotFound();

        _context.CategoryArticles.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}