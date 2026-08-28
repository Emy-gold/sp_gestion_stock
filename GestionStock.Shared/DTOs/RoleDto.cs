namespace GestionStock.Shared.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class RoleCreateDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
