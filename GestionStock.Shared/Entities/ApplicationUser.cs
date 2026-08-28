namespace GestionStock.Shared.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set;  }
        public string Nom {  get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string? Telephone { get; set; }

        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        //Navigation
        public ICollection<Operation> OperationsCreees { get; set; } = new List<Operation>();
        public ICollection<Operation> OperationsModifiees { get; set; } = new List<Operation>();

    }
}
