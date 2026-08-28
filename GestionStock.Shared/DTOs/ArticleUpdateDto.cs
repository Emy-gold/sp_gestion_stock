namespace GestionStock.Shared.DTOs;

public class ArticleUpdateDto
{
    public string Reference { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? CodeBarre { get; set; }
    public bool Actif { get; set; }
    public int CategoryArticleId { get; set; }
    public Dictionary<string, string>? AttributeValues { get; set; }
}