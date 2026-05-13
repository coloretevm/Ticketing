using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tecnidro.Ticketing.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketingDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Asunto = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    CuerpoOriginal = table.Column<string>(type: "TEXT", nullable: false),
                    EmailCliente = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    NombreCliente = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Prioridad = table.Column<int>(type: "INTEGER", nullable: false),
                    AsignadoAUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CerradoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ResolucionTexto = table.Column<string>(type: "TEXT", nullable: true),
                    MessageIdOutlook = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_AsignadoAUserId",
                        column: x => x.AsignadoAUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    AsuntoEnviado = table.Column<string>(type: "TEXT", nullable: false),
                    Destinatario = table.Column<string>(type: "TEXT", nullable: false),
                    CreadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAdjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreArchivo = table.Column<string>(type: "TEXT", nullable: false),
                    Ruta = table.Column<string>(type: "TEXT", nullable: false),
                    TamanoBytes = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAdjuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAdjuntos_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketComentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    CreadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComentarios_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketComentarios_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_TicketId",
                table: "EmailLogs",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAdjuntos_TicketId",
                table: "TicketAdjuntos",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComentarios_TicketId",
                table: "TicketComentarios",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComentarios_UserId",
                table: "TicketComentarios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AsignadoAUserId",
                table: "Tickets",
                column: "AsignadoAUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Numero",
                table: "Tickets",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "TicketAdjuntos");

            migrationBuilder.DropTable(
                name: "TicketComentarios");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");
        }
    }
}
