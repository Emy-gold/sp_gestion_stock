using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetailOperationsController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public DetailOperationsController(GestionStockDbContext context)
    {
        _context = context;
    }

    // GET: api/detailoperations/byoperation/5
    [HttpGet("byoperation/{operationId}")]
    public async Task<ActionResult<IEnumerable<DetailOperationDto>>> GetByOperation(int operationId)
    {
        var details = await _context.DetailOperations
            .Include(d => d.Article)
            .Where(d => d.OperationId == operationId)
            .Select(d => new DetailOperationDto
            {
                Id = d.Id,
                ArticleId = d.ArticleId,
                ArticleDesignation = d.Article.Designation,
                Quantite = d.Quantite,
                Emplacement = d.Emplacement,
                Remarque = d.Remarque
            })
            .ToListAsync();

        return Ok(details);
    }

    // GET: api/detailoperations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetailOperationDto>> GetById(int id)
    {
        var detail = await _context.DetailOperations
            .Include(d => d.Article)
            .Where(d => d.Id == id)
            .Select(d => new DetailOperationDto
            {
                Id = d.Id,
                ArticleId = d.ArticleId,
                ArticleDesignation = d.Article.Designation,
                Quantite = d.Quantite,
                Emplacement = d.Emplacement,
                Remarque = d.Remarque
            })
            .FirstOrDefaultAsync();

        if (detail is null)
            return NotFound();

        return Ok(detail);
    }

    // PUT: api/detailoperations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DetailOperationCreateDto dto)
    {
        var detail = await _context.DetailOperations.FindAsync(id);
        if (detail is null)
            return NotFound();

        var articleExists = await _context.Articles.AnyAsync(a => a.Id == dto.ArticleId);
        if (!articleExists)
            return BadRequest("ArticleId invalide.");

        detail.ArticleId = dto.ArticleId;
        detail.Quantite = dto.Quantite;
        detail.Emplacement = dto.Emplacement;
        detail.Remarque = dto.Remarque;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/detailoperations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var detail = await _context.DetailOperations.FindAsync(id);
        if (detail is null)
            return NotFound();

        _context.DetailOperations.Remove(detail);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}