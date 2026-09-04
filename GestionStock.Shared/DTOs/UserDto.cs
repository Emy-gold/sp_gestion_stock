namespace GestionStock.Shared.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public int? RoleId { get; set; }
    public string? RoleNom { get; set; }
    public string? MotDePasse { get; set; }
}

public class UserCreateDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string MotDePasse { get; set; } = string.Empty;
    public int? RoleId { get; set; }
}
