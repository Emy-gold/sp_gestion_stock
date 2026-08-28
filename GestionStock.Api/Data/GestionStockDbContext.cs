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

    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// ApplicationUser -> Role
		modelBuilder.Entity<ApplicationUser>()
			.HasOne(u => u.Role)
			.WithMany(r => r.Users)
			.HasForeignKey(u => u.RoleId)
			.OnDelete(DeleteBehavior.SetNull);

		// CategoryArticle : auto-reference (parent/enfants)
		modelBuilder.Entity<CategoryArticle>()
			.HasOne(c => c.Parent)
			.WithMany(c => c.Enfants)
			.HasForeignKey(c => c.ParentId)
			.OnDelete(DeleteBehavior.Restrict);

		var dictComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, string>>(
			(c1, c2) => c1.SequenceEqual(c2),
			c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
			c => c.ToDictionary(entry => entry.Key, entry => entry.Value)
		);

		// Colonne JSON native EF core 8
		modelBuilder.Entity<CategoryArticle>()
			.Property(c => c.Attributes)
			.HasColumnType("nvarchar(max)")
			.HasConversion(
				v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
				v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
			)
			.Metadata.SetValueComparer(dictComparer);


		// CategoryOperation : meme logique Json
		modelBuilder.Entity<CategoryOperation>()
			.Property(c => c.Attributes)
			.HasColumnType("nvarchar(max)")
			.HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            )
			.Metadata.SetValueComparer(dictComparer);


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

		// Article.AttributeValues : JSON column
		modelBuilder.Entity<Article>()
			.Property(a => a.AttributeValues)
			.HasColumnType("nvarchar(max)")
			.HasConversion(
				v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
				v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
			)
			.Metadata.SetValueComparer(dictComparer);

		//Operation -> CategoryOperation
		modelBuilder.Entity<Operation>()
			.HasOne(o => o.CategoryOperation)
			.WithMany(c => c.Operations)
			.HasForeignKey(o => o.CategoryOperationId)
			.OnDelete(DeleteBehavior.Restrict);


		modelBuilder.Entity<Article>().HasIndex(a => a.Reference).IsUnique();
		modelBuilder.Entity<Article>().HasIndex(a => a.CodeBarre);
		modelBuilder.Entity<Operation>().HasIndex(o => o.Numero).IsUnique();

		modelBuilder.Entity<Article>().Property(a => a.StockActuel).HasPrecision(18, 2);
		modelBuilder.Entity<DetailOperation>().Property(d => d.Quantite).HasPrecision(18, 2);
	}


}
