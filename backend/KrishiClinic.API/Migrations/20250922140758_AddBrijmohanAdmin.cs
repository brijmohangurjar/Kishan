using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrishiClinic.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBrijmohanAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "Email", "Name", "Password" },
                values: new object[] { "brijmohangurjar48@gmail.com", "Brijmohan Gurjar", "$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "Email", "Name", "Password" },
                values: new object[] { "admin@krishiclinic.com", "Admin User", "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy" });
        }
    }
}
