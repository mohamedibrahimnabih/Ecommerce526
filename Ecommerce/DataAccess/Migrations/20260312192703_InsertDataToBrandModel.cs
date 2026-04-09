using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataToBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (Name, Logo, Status) values ('Samsung', 'Samsung.png', 1); insert into Brands (Name, Logo, Status) values ('Apple', 'Apple.png', 1); insert into Brands (Name, Logo, Status) values ('OPPO', 'OPPO.png', 1); insert into Brands (Name, Logo, Status) values ('DELL', 'DELL.png', 1); insert into Brands (Name, Logo, Status) values ('HP', 'HP.png', 1); insert into Brands (Name, Logo, Status) values ('ASUS', 'ASUS.png', 1); insert into Brands (Name, Logo, Status) values ('Lenovo', 'Lenovo.png', 0); insert into Brands (Name, Logo, Status) values ('BUES', 'BUES.png', 1); insert into Brands (Name, Logo, Status) values ('Cover', 'Cover.png', 1); insert into Brands (Name, Logo, Status) values ('Charger', 'Charger.png', 1); ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Brands");
        }
    }
}
