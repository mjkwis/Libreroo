using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libreroo.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedCatalogEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "authors",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "George Orwell" },
                    { 2, "Jane Austen" },
                    { 3, "Fyodor Dostoevsky" },
                    { 4, "Harper Lee" },
                    { 5, "J.R.R. Tolkien" }
                });

            migrationBuilder.InsertData(
                table: "books",
                columns: new[] { "Id", "Title", "AuthorId", "AvailableCopies" },
                values: new object[,]
                {
                    { 1, "1984", 1, 6 },
                    { 2, "Animal Farm", 1, 4 },
                    { 3, "Pride and Prejudice", 2, 5 },
                    { 4, "Sense and Sensibility", 2, 3 },
                    { 5, "Crime and Punishment", 3, 4 },
                    { 6, "The Brothers Karamazov", 3, 2 },
                    { 7, "To Kill a Mockingbird", 4, 5 },
                    { 8, "The Hobbit", 5, 8 },
                    { 9, "The Fellowship of the Ring", 5, 6 },
                    { 10, "The Two Towers", 5, 6 },
                    { 11, "The Return of the King", 5, 6 }
                });

            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"authors"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "authors"), 1),
                    true);
                """);

            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"books"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "books"), 1),
                    true);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "books"
                WHERE "Id" IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
                """);

            migrationBuilder.Sql(
                """
                DELETE FROM "authors"
                WHERE "Id" IN (1, 2, 3, 4, 5);
                """);

            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"authors"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "authors"), 1),
                    true);
                """);

            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"books"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "books"), 1),
                    true);
                """);
        }
    }
}
