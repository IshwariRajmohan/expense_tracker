using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "UserProfiles",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "Expenses",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDate",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApprovalHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", nullable: false),
                    ExpenseId = table.Column<string>(type: "varchar(50)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FreezeDateSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FreezeDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreezeDateSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorporateCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemMode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ManagerId",
                table: "UserProfiles",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_UserProfiles_ManagerId",
                table: "UserProfiles",
                column: "ManagerId",
                principalTable: "UserProfiles",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_UserProfiles_ManagerId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "ApprovalHistory");

            migrationBuilder.DropTable(
                name: "FreezeDateSettings");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_ManagerId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Expenses");
        }
    }
}
