namespace GestionStock.Shared.Entities;

public class CategoryArticle 
{
    public int  Id { get; set; }
    public string Nom {  get; set; }
    public string? Description { get; set; }
    public string? Image {  get; set; }

    //Json column
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public int? ParentId { get; set; }

    public CategoryArticle? Parent { get; set; }
    public ICollection<CategoryArticle> Enfants { get; set; } = new List<CategoryArticle>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}