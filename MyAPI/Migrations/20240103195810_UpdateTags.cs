using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Products_ProductId",
                schema: "MySchema",
                table: "Tag");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                schema: "MySchema",
                table: "Tag",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Products_ProductId",
                schema: "MySchema",
                table: "Tag",
                column: "ProductId",
                principalSchema: "MySchema",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Products_ProductId",
                schema: "MySchema",
                table: "Tag");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                schema: "MySchema",
                table: "Tag",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Products_ProductId",
                schema: "MySchema",
                table: "Tag",
                column: "ProductId",
                principalSchema: "MySchema",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
