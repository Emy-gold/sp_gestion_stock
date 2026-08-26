using GestionStock.Api.Data;
using GestionStock.Shared.DTOs;
using GestionStock.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
	private readonly GestionStockDbContext _context;

	public ArticlesController(GestionStockDbContext context)
	{
		_context = context;
	}

	// GET: api/articles
	[HttpGet]
	public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles()
	{
		var articles = await _context.Articles
			.Include(a => a.CategoryArticle)
			.Select(a => new ArticleDto
			{
				Id = a.Id,
				Reference = a.Reference,
				Designation = a.Designation,
				Description = a.Description,
				Image = a.Image,
				CodeBarre = a.CodeBarre,
				StockActuel = a.StockActuel,
				Actif = a.Actif,
				CategoryArticleId = a.CategoryArticleId,
				CategoryArticleNom = a.CategoryArticle.Nom
			})
			.ToListAsync();

		return Ok(articles);
	}

	// GET: api/articles/5
	[HttpGet("{id}")]
	public async Task<ActionResult<ArticleDto>> GetArticle(int id)
	{
		var article = await _context.Articles
			.Include(a => a.CategoryArticle)
			.Where(a => a.Id == id)
			.Select(a => new ArticleDto
			{
				Id = a.Id,
				Reference = a.Reference,
				Designation = a.Designation,
				Description = a.Description,
				Image = a.Image,
				CodeBarre = a.CodeBarre,
				StockActuel = a.StockActuel,
				Actif = a.Actif,
				CategoryArticleId = a.CategoryArticleId,
				CategoryArticleNom = a.CategoryArticle.Nom
			})
			.FirstOrDefaultAsync();

		if (article is null)
			return NotFound();

		return Ok(article);
	}

	// POST: api/articles
	[HttpPost]
	public async Task<ActionResult<ArticleDto>> CreateArticle(ArticleCreateDto dto)
	{
		var refExists = await _context.Articles.AnyAsync(a => a.Reference.ToLower() == dto.Reference.Trim().ToLower());
		if (refExists)
			return BadRequest($"Un article avec la référence '{dto.Reference}' existe déjà.");

		var category = await _context.CategoryArticles.FindAsync(dto.CategoryArticleId);
		if (category == null)
			return BadRequest("Catégorie sélectionnée invalide.");

		var article = new Article
		{
			Reference = dto.Reference.Trim(),
			Designation = dto.Designation.Trim(),
			Description = dto.Description,
			Image = dto.Image,
			CodeBarre = dto.CodeBarre,
			CategoryArticleId = dto.CategoryArticleId,
			StockActuel = 0,
			Actif = true
		};

		_context.Articles.Add(article);
		await _context.SaveChangesAsync();

		var articleDto = new ArticleDto
		{
			Id = article.Id,
			Reference = article.Reference,
			Designation = article.Designation,
			Description = article.Description,
			Image = article.Image,
			CodeBarre = article.CodeBarre,
			StockActuel = article.StockActuel,
			Actif = article.Actif,
			CategoryArticleId = article.CategoryArticleId,
			CategoryArticleNom = category.Nom
		};

		return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, articleDto);
	}

	// PUT: api/articles/5
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateArticle(int id, ArticleUpdateDto dto)
	{
		var article = await _context.Articles.FindAsync(id);
		if (article is null)
			return NotFound("Article introuvable.");

		var refExists = await _context.Articles.AnyAsync(a => a.Reference.ToLower() == dto.Reference.Trim().ToLower() && a.Id != id);
		if (refExists)
			return BadRequest($"Un autre article avec la référence '{dto.Reference}' existe déjà.");

		var categoryExists = await _context.CategoryArticles.AnyAsync(c => c.Id == dto.CategoryArticleId);
		if (!categoryExists)
			return BadRequest("Catégorie sélectionnée invalide.");

		article.Reference = dto.Reference.Trim();
		article.Designation = dto.Designation.Trim();
		article.Description = dto.Description;
		article.Image = dto.Image;
		article.CodeBarre = dto.CodeBarre;
		article.Actif = dto.Actif;
		article.CategoryArticleId = dto.CategoryArticleId;

		await _context.SaveChangesAsync();
		return NoContent();
	}

	// DELETE: api/articles/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteArticle(int id)
	{
		var article = await _context.Articles.FindAsync(id);
		if (article is null)
			return NotFound();

		_context.Articles.Remove(article);
		await _context.SaveChangesAsync();
		return NoContent();
	}
}