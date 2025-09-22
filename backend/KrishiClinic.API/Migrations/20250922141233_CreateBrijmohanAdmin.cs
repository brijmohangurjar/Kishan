using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrishiClinic.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateBrijmohanAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$rQZ8K3tXvL7hM9nP2qR1te8K5wF3sA6bC9dE1fG4hI7jK0lM3nP6q");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi");
        }
    }
}
