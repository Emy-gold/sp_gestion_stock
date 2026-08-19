namespace GestionStock.Shared.Entities

{
    public class Operation
    {

        private int Id {  get; set; }
        private string Numero { get; set; } = string.Empty;
        public DateTime DateOperation { get; set; }
        public string? Observation { get; set; }

        // Champs libres réutilisables
        public string? Ch01 { get; set; }
        public string? Ch02 { get; set; }
        public string? Ch03 { get; set; }
        public string? Ch04 { get; set; }
        public string? Ch05 { get; set; }
        public string? Ch06 { get; set; }
        public string? Ch07 { get; set; }
        public string? Ch08 { get; set; }
        public string? Ch09 { get; set; }
        public string? Ch10 { get; set; }

        public int? IdParentOperation { get; set; }
        public Operation? OperationParent { get; set; }
        public ICollection<Operation> SousOperations { get; set; } = new List<Operation>();

        public int CreePar {  get; set; }
        public ApplicationUser CreeParUser { get; set; } = null!;
        public DateTime CreeLe { get; set; }

        public int? ModifiePar { get; set; }
        public ApplicationUser? ModifieParUser { get; set; }
        public DateTime? ModifieLe { get; set; }


        public int CategoryOperationId { get; set; }
        public CategoryOperation CategoryOperation { get; set; } = null!;

        public int? FournisseurId {  get; set; }
        public Fournisseur? Fournisseur { get; set; }

        public ICollection<DetailOperation> DetailOperations { get; set; } = new List<DetailOperation>;
    }
}
