namespace GestionStock.Shared.DTOs;

public class DetailOperationCreateDto
{
    public int ArticleId { get; set; }
    public decimal Quantite { get; set; }
    public string? Emplacement { get; set; }
    public string? Remarque { get; set; }
}

public class DetailOperationDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleDesignation { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public string? Emplacement { get; set; }
    public string? Remarque { get; set; }
}