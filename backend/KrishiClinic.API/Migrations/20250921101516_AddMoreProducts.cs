using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KrishiClinic.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$pihd6kYVNUS5GMpfElsIeeOyTOsZou4QVCcka6u5m8A1fbpzVNOF." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, "Fungicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fungicide for controlling leaf spot, rust and powdery mildew in various crops.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Syngenta - Tilt", 650m, 60, null },
                    { 5, "Fertilizers", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Diammonium phosphate fertilizer for promoting root development and flowering.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "DAP Fertilizer - 18:46:0", 320m, 80, null },
                    { 6, "Fertilizers", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High nitrogen content urea fertilizer for vegetative growth and green foliage.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Urea - 46:0:0", 280m, 120, null },
                    { 7, "Herbicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-selective herbicide for controlling weeds in agricultural fields.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Roundup - Glyphosate", 580m, 45, null },
                    { 8, "Seeds", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium quality chilli seeds with high yield potential and disease resistance.", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop", true, "Organic Seeds - Chilli", 350m, 90, null },
                    { 9, "Fertilizers", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Muriate of Potash for improving fruit quality and disease resistance.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "MOP Fertilizer - 0:0:60", 380m, 70, null },
                    { 10, "Insecticides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Systemic insecticide for controlling aphids, jassids, and whiteflies in cotton and vegetables.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Imidacloprid - 17.8% SL", 520m, 55, null },
                    { 11, "Seeds", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-yielding brinjal seeds with excellent fruit quality and market demand.", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop", true, "Organic Seeds - Brinjal", 420m, 85, null },
                    { 12, "Fungicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Contact fungicide for controlling early and late blight in potato and tomato crops.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Mancozeb - 75% WP", 480m, 65, null },
                    { 13, "Fertilizers", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Single Super Phosphate for root development and early crop establishment.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "SSP Fertilizer - 0:16:0", 340m, 95, null },
                    { 14, "Herbicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Selective herbicide for controlling broadleaf weeds in wheat and rice fields.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "2,4-D Herbicide", 620m, 40, null },
                    { 15, "Seeds", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium onion seeds with high bulb weight and good storage quality.", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop", true, "Organic Seeds - Onion", 380m, 75, null },
                    { 16, "Insecticides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organophosphate insecticide for controlling soil and foliar pests in various crops.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Chlorpyrifos - 20% EC", 580m, 50, null },
                    { 17, "Fertilizers", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Balanced complex fertilizer for flowering and fruiting crops like tomato and brinjal.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Complex Fertilizer - 12:32:16", 420m, 60, null },
                    { 18, "Fungicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Systemic fungicide for controlling sheath blight, blast and other fungal diseases.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Carbendazim - 50% WP", 550m, 45, null },
                    { 19, "Seeds", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-yielding okra seeds with tender, long pods suitable for market and home use.", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop", true, "Organic Seeds - Okra", 320m, 80, null },
                    { 20, "Herbicides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Contact herbicide for controlling weeds in plantation crops and non-crop areas.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Paraquat - 24% SL", 680m, 35, null },
                    { 21, "Seeds", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium cucumber seeds with high yield, disease resistance and excellent fruit quality.", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=300&h=300&fit=crop", true, "Organic Seeds - Cucumber", 290m, 70, null },
                    { 22, "Insecticides", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Synthetic pyrethroid insecticide for controlling lepidopteran pests in cotton and vegetables.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Lambda Cyhalothrin - 5% EC", 720m, 40, null },
                    { 23, "Micronutrients", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Essential micronutrients including zinc, boron, iron, and manganese for healthy crop growth.", "https://images.unsplash.com/photo-1586771107445-d3ca888129ce?w=300&h=300&fit=crop", true, "Micronutrient Mixture", 890m, 25, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 9, 21, 9, 0, 35, 824, DateTimeKind.Utc).AddTicks(6280), "$2a$11$crH9Z5Rc/MIZ2myHrnxrIufD8P4X/fDSpAZyJvH2jJ.XnKplooJfy" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 9, 0, 35, 466, DateTimeKind.Utc).AddTicks(9036));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 9, 0, 35, 466, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 21, 9, 0, 35, 466, DateTimeKind.Utc).AddTicks(9239));
        }
    }
}
