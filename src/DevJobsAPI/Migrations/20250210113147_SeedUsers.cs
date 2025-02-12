using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevJobsAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1L, "alice@example.com", "Alice Johnson", "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f", 1L },
                    { 2L, "bob@example.com", "Bob Smith", "c6ba91b90d922e159893f46c387e5dc1b3dc5c101a5a4522f03b987177a24a91", 2L },
                    { 3L, "charlie@example.com", "Charlie Admin", "713bfda78870bf9d1b261f565286f85e97ee614efe5f0faf7c34e7ca4f65baca", 3L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L);
        }
    }
}
