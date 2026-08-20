using GestionStock.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Api.Data;

public class GestionStockDbContext : DbContext
{
	public GestionStockDbContext(DbContextOptions<GestionStockDbContext> options) : base(options)
	{}

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Fournisseur> Fournisseurs => Set<Fournisseur>();
    public DbSet<CategoryArticle> CategoryArticles => Set<CategoryArticle>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<CategoryOperation> CategoryOperations => Set<CategoryOperation>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<DetailOperation> DetailOperations => Set<DetailOperation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// CategoryArticle : auto-reference (parent/enfants)
		modelBuilder.Entity<CategoryArticle>()
			.HasOne(c => c.Parent)
			.WithMany(c => c.Enfants)
			.HasForeignKey(c => c.ParentId)
			.OnDelete(DeleteBehavior.Restrict);

		// Colonne JSON native EF core 8
		modelBuilder.Entity<CategoryArticle>()
			.Property(c => c.Attributes)
			.HasColumnType("nvarchar(max)")
			.HasConversion(
				v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
				v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
			);


		// CategoryOperation : meme logique Json
		modelBuilder.Entity<CategoryOperation>()
			.Property(c => c.Attributes)
			.HasColumnType("nvarchar(max)")
			.HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );


		//Operation : auto-reference 
		modelBuilder.Entity<Operation>()
			.HasOne(o => o.OperationParent)
			.WithMany(o => o.SousOperations)
			.HasForeignKey(o => o.IdParentOperation)
			.OnDelete(DeleteBehavior.Restrict);

		//Operation -> ApplicationUser
		modelBuilder.Entity<Operation>()
			.HasOne(o => o.CreeParUser)
			.WithMany(u => u.OperationsCreees)
			.HasForeignKey(o => o.CreePar)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Operation>()
			.HasOne(o => o.ModifieParUser)
			.WithMany(u => u.OperationsModifiees)
			.HasForeignKey(o => o.ModifiePar)
			.OnDelete(DeleteBehavior.Restrict);

		//Operation -> Fournisseur
		modelBuilder.Entity<Operation>()
			.HasOne(o => o.Fournisseur)
			.WithMany(f => f.Operations)
			.HasForeignKey(o => o.FournisseurId)
			.OnDelete(DeleteBehavior.Restrict);

		//Operation -> DetailOperation
		modelBuilder.Entity<DetailOperation>()
			.HasOne(d => d.Operation)
			.WithMany(o => o.DetailOperations)
			.HasForeignKey(d => d.OperationId)
			.OnDelete(DeleteBehavior.Cascade);

		//Article -> DetailOperation
		modelBuilder.Entity<DetailOperation>()
			.HasOne(d => d.Article)
			.WithMany(a => a.DetailOperations)
			.HasForeignKey(d => d.ArticleId)
			.OnDelete(DeleteBehavior.Restrict);

		//Article -> CategoryArticle
		modelBuilder.Entity<Article>()
			.HasOne(a => a.CategoryArticle)
			.WithMany(c => c.Articles)
			.HasForeignKey(a => a.CategoryArticleId)
			.OnDelete(DeleteBehavior.Restrict);

		//Operation -> CategoryOperation
		modelBuilder.Entity<Operation>()
			.HasOne(o => o.CategoryOperation)
			.WithMany(c => c.Operations)
			.HasForeignKey(o => o.CategoryOperationId)
			.OnDelete(DeleteBehavior.Restrict);


		modelBuilder.Entity<Article>().HasIndex(a => a.Reference).IsUnique();
		modelBuilder.Entity<Article>().HasIndex(a => a.CodeBarre);
		modelBuilder.Entity<Operation>().HasIndex(o => o.Numero).IsUnique();

	}


}
