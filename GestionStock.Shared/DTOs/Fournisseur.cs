namespace GestionStock.Shared.DTOs;

public class FournisseurDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public bool Actif { get; set; }
}

public class FournisseurCreateDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
}

public class FournisseurUpdateDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public bool Actif { get; set; }
}