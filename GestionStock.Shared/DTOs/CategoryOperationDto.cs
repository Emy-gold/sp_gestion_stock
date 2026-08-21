namespace GestionStock.Shared.DTOs;

public class CategoryOperationDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}

public class CategoryOperationCreateDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}