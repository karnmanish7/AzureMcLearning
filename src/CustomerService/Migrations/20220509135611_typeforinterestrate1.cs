using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerService.Migrations
{
    public partial class typeforinterestrate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "loanDuration",
                table: "Loans",
                newName: "LoanDuration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoanDuration",
                table: "Loans",
                newName: "loanDuration");
        }
    }
}
