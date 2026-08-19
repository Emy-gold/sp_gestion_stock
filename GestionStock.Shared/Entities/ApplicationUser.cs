namespace GestionStock.Shared.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set;  }
        public string Nom {  get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string? Telephone { get; set; }

        //Navigation
        public ICollection<Operation> OperationsCreees { get; set; } = new List<Operation>();
        public ICollection<Operation> OperationModifiees { get; set; } = new List<Operation>();

    }
}
