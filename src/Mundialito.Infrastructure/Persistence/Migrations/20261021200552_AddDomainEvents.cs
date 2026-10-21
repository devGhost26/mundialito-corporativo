using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mundialito.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_OccurredOnUtc",
                table: "DomainEvents",
                column: "OccurredOnUtc");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_TraceId",
                table: "DomainEvents",
                column: "TraceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents");
        }
    }
}
