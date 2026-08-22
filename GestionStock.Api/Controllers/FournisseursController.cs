using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FournisseursController : ControllerBase
{
    private readonly GestionStockDbContext _context;

    public FournisseursController(GestionStockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FournisseurDto>>> GetAll()
    {
        var fournisseurs = await _context.Fournisseurs
            .Select(f => new FournisseurDto
            {
                Id = f.Id,
                Nom = f.Nom,
                Telephone = f.Telephone,
                Email = f.Email,
                Adresse = f.Adresse,
                Actif = f.Actif
            })
            .ToListAsync();

        return Ok(fournisseurs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FournisseurDto>> GetById(int id)
    {
        var fournisseur = await _context.Fournisseurs.FindAsync(id);
        if (fournisseur is null)
            return NotFound();

        return Ok(new FournisseurDto
        {
            Id = fournisseur.Id,
            Nom = fournisseur.Nom,
            Telephone = fournisseur.Telephone,
            Email = fournisseur.Email,
            Adresse = fournisseur.Adresse,
            Actif = fournisseur.Actif
        });
    }

    [HttpPost]
    public async Task<ActionResult<FournisseurDto>> Create(FournisseurCreateDto dto)
    {
        var fournisseur = new Fournisseur
        {
            Nom = dto.Nom,
            Telephone = dto.Telephone,
            Email = dto.Email,
            Adresse = dto.Adresse,
            Actif = true
        };

        _context.Fournisseurs.Add(fournisseur);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = fournisseur.Id }, fournisseur);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FournisseurUpdateDto dto)
    {
        var fournisseur = await _context.Fournisseurs.FindAsync(id);
        if (fournisseur is null)
            return NotFound();

        fournisseur.Nom = dto.Nom;
        fournisseur.Telephone = dto.Telephone;
        fournisseur.Email = dto.Email;
        fournisseur.Adresse = dto.Adresse;
        fournisseur.Actif = dto.Actif;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hasOperations = await _context.Operations.AnyAsync(o => o.FournisseurId == id);
        if (hasOperations)
            return BadRequest("Impossible de supprimer : ce fournisseur a des opérations liées.");

        var fournisseur = await _context.Fournisseurs.FindAsync(id);
        if (fournisseur is null)
            return NotFound();

        _context.Fournisseurs.Remove(fournisseur);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}