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
		var categoryExists = await _context.CategoryArticles.AnyAsync(c => c.Id == dto.CategoryArticleId);
		if (!categoryExists)
			return BadRequest("CategoryArticleId invalide.");

		var article = new Article
		{
			Reference = dto.Reference,
			Designation = dto.Designation,
			Description = dto.Description,
			Image = dto.Image,
			CodeBarre = dto.CodeBarre,
			CategoryArticleId = dto.CategoryArticleId,
			StockActuel = 0,
			Actif = true
		};

		_context.Articles.Add(article);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
	}

	// PUT: api/articles/5
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateArticle(int id, ArticleUpdateDto dto)
	{
		var article = await _context.Articles.FindAsync(id);
		if (article is null)
			return NotFound();

		article.Reference = dto.Reference;
		article.Designation = dto.Designation;
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