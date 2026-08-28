namespace GestionStock.Shared.Entities;


public class Article
{
	public int Id { get; set; }
	public string Reference { get; set; } = string.Empty;
	public string Designation {  get; set; } = string.Empty;
	public string? Description {  get; set; }
	public string? Image {  get; set; }
	public string? CodeBarre { get; set; }
	public decimal StockActuel { get; set; }
	public bool Actif { get; set; } = true;
	public Dictionary<string, string> AttributeValues { get; set; } = new();

	public int CategoryArticleId { get ; set; }
	public CategoryArticle CategoryArticle { get; set; } = null;
	public ICollection<DetailOperation> DetailOperations { get; set; } = new List<DetailOperation>();
}
