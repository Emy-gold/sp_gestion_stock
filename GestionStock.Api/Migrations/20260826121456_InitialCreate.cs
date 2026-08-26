using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStock.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attributes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryArticles_CategoryArticles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CategoryArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attributes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournisseurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeBarre = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StockActuel = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    CategoryArticleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_CategoryArticles_CategoryArticleId",
                        column: x => x.CategoryArticleId,
                        principalTable: "CategoryArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateOperation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch01 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch02 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch03 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch04 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch05 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch06 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch07 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch08 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch09 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ch10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdParentOperation = table.Column<int>(type: "int", nullable: true),
                    CreePar = table.Column<int>(type: "int", nullable: false),
                    CreeLe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiePar = table.Column<int>(type: "int", nullable: true),
                    ModifieLe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryOperationId = table.Column<int>(type: "int", nullable: false),
                    FournisseurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_ApplicationUsers_CreePar",
                        column: x => x.CreePar,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_ApplicationUsers_ModifiePar",
                        column: x => x.ModifiePar,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_CategoryOperations_CategoryOperationId",
                        column: x => x.CategoryOperationId,
                        principalTable: "CategoryOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Operations_Operations_IdParentOperation",
                        column: x => x.IdParentOperation,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetailOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantite = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Emplacement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarque = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailOperations_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetailOperations_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryArticleId",
                table: "Articles",
                column: "CategoryArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CodeBarre",
                table: "Articles",
                column: "CodeBarre");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Reference",
                table: "Articles",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryArticles_ParentId",
                table: "CategoryArticles",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailOperations_ArticleId",
                table: "DetailOperations",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailOperations_OperationId",
                table: "DetailOperations",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CategoryOperationId",
                table: "Operations",
                column: "CategoryOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CreePar",
                table: "Operations",
                column: "CreePar");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_FournisseurId",
                table: "Operations",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_IdParentOperation",
                table: "Operations",
                column: "IdParentOperation");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ModifiePar",
                table: "Operations",
                column: "ModifiePar");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_Numero",
                table: "Operations",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailOperations");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "CategoryArticles");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "CategoryOperations");

            migrationBuilder.DropTable(
                name: "Fournisseurs");
        }
    }
}
