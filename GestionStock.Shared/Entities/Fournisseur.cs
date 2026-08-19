namespace GestionStock.Shared.Entities;

public class Fournisseur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public bool Actif { get; set; } = true;

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}