namespace GestionStock.Shared.Entities;

public class CategoryOperation
{
	public int Id { get; set; }
	public string Nom {  get; set; } = string.Empty;
	public string? Description { get; set; }
	public Dictionary<string, string>? Attributes { get; set; }

	public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}
