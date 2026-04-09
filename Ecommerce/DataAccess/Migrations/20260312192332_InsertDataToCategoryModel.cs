using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataToCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (Name, Status) values ('Mobiles', 0); insert into Categories (Name, Status) values ('Computers', 1); insert into Categories (Name, Status) values ('Laptops', 1); insert into Categories (Name, Status) values ('Cameras', 1); insert into Categories (Name, Status) values ('Accessories', 0); ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Categories");
        }
    }
}
