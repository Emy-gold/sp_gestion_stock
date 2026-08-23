namespace GestionStock.Shared.DTOs;

public class OperationCreateDto
{
	public string Numero { get; set; } = string.Empty;
	public DateTime DateOperation { get; set; }
	public string? Observation { get; set; }
	public int CategoryOperationId { get; set; }
	public int? FournisseurId { get; set; }
	public int? IdParentOperation { get; set; }
	public List<DetailOperationCreateDto> Details { get; set; } = new();
}

public class OperationDto
{
	public int Id { get; set; }
	public string Numero { get; set; } = string.Empty;
	public DateTime DateOperation { get; set; }
	public string? Observation { get; set; }
	public int CategoryOperationId { get; set; }
	public string CategoryOperationNom { get; set; } = string.Empty;
	public int? FournisseurId { get; set; }
	public string? FournisseurNom { get; set; }
	public string CreeParNom { get; set; } = string.Empty;
	public DateTime CreeLe { get; set; }
	public List<DetailOperationDto> Details { get; set; } = new();
}