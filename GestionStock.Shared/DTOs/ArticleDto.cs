namespace GestionStock.Shared.DTOs;

public class ArticleDto
{
	public int Id { get; set; }
	public string Reference { get; set; } = string.Empty;
	public string Designation { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Image { get; set; }
	public string? CodeBarre { get; set; }
	public decimal StockActuel { get; set; }
	public bool Actif { get; set; }
	public int CategoryArticleId { get; set; }
	public string? CategoryArticleNom { get; set; }
}