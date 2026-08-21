namespace  GestionStock.Shared.Entities;

public class DetailOperation
{
	
	public int Id { get; set; }
	public decimal Quantite {  get; set; }
	public string? Emplacement { get; set; }
	public string? Remarque { get; set; }

	public int OperationId { get; set; }
	public Operation Operation { get; set; } = null!;

	public int ArticleId { get; set; }
	public Article Article { get; set; } = null!;
}
