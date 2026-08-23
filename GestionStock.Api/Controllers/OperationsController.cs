using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public OperationsController(GestionStockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetAll()
    {
        var operations = await _context.Operations
            .Include(o => o.CategoryOperation)
            .Include(o => o.Fournisseur)
            .Include(o => o.CreeParUser)
            .Include(o => o.DetailOperations).ThenInclude(d => d.Article)
            .OrderByDescending(o => o.DateOperation)
            .Select(o => MapToDto(o))
            .ToListAsync();

        return Ok(operations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OperationDto>> GetById(int id)
    {
        var operation = await _context.Operations
            .Include(o => o.CategoryOperation)
            .Include(o => o.Fournisseur)
            .Include(o => o.CreeParUser)
            .Include(o => o.DetailOperations).ThenInclude(d => d.Article)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (operation is null)
            return NotFound();

        return Ok(MapToDto(operation));
    }

    // POST: api/operations
    // TODO: CreePar est en dur (=1) tant que l'authentification n'est pas en place
    [HttpPost]
    public async Task<ActionResult<OperationDto>> Create(OperationCreateDto dto)
    {
        if (dto.Details is null || dto.Details.Count == 0)
            return BadRequest("Une opération doit contenir au moins un détail.");

        var categoryExists = await _context.CategoryOperations.AnyAsync(c => c.Id == dto.CategoryOperationId);
        if (!categoryExists)
            return BadRequest("CategoryOperationId invalide.");

        var articleIds = dto.Details.Select(d => d.ArticleId).Distinct().ToList();
        var validArticleCount = await _context.Articles.CountAsync(a => articleIds.Contains(a.Id));
        if (validArticleCount != articleIds.Count)
            return BadRequest("Un ou plusieurs ArticleId sont invalides.");

        var operation = new Operation
        {
            Numero = dto.Numero,
            DateOperation = dto.DateOperation,
            Observation = dto.Observation,
            CategoryOperationId = dto.CategoryOperationId,
            FournisseurId = dto.FournisseurId,
            IdParentOperation = dto.IdParentOperation,
            CreePar = 1, // TODO: remplacer par l'utilisateur connecté
            CreeLe = DateTime.UtcNow
        };

        foreach (var detailDto in dto.Details)
        {
            operation.DetailOperations.Add(new DetailOperation
            {
                ArticleId = detailDto.ArticleId,
                Quantite = detailDto.Quantite,
                Emplacement = detailDto.Emplacement,
                Remarque = detailDto.Remarque
            });
        }

        _context.Operations.Add(operation);
        await _context.SaveChangesAsync();

        var created = await _context.Operations
            .Include(o => o.CategoryOperation)
            .Include(o => o.Fournisseur)
            .Include(o => o.CreeParUser)
            .Include(o => o.DetailOperations).ThenInclude(d => d.Article)
            .FirstAsync(o => o.Id == operation.Id);

        return CreatedAtAction(nameof(GetById), new { id = operation.Id }, MapToDto(created));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var operation = await _context.Operations.FindAsync(id);
        if (operation is null)
            return NotFound();

        _context.Operations.Remove(operation);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static OperationDto MapToDto(Operation o) => new()
    {
        Id = o.Id,
        Numero = o.Numero,
        DateOperation = o.DateOperation,
        Observation = o.Observation,
        CategoryOperationId = o.CategoryOperationId,
        CategoryOperationNom = o.CategoryOperation.Nom,
        FournisseurId = o.FournisseurId,
        FournisseurNom = o.Fournisseur?.Nom,
        CreeParNom = $"{o.CreeParUser.Prenom} {o.CreeParUser.Nom}",
        CreeLe = o.CreeLe,
        Details = o.DetailOperations.Select(d => new DetailOperationDto
        {
            Id = d.Id,
            ArticleId = d.ArticleId,
            ArticleDesignation = d.Article.Designation,
            Quantite = d.Quantite,
            Emplacement = d.Emplacement,
            Remarque = d.Remarque
        }).ToList()
    };
}