using System.Text.Json.Serialization;

namespace GestionStock.Shared.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
