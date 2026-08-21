namespace GestionStock.Shared.DTOs;

public class CategoryArticleDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
    public int? ParentId { get; set; }
    public string? ParentNom { get; set; }
}

public class CategoryArticleCreateDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
    public int? ParentId { get; set; }
}