using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfTask2.Migrations
{
    public partial class FirstMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    PageId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    AddedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now')"),
                    Content = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    UrlName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.PageId);
                });

            migrationBuilder.CreateTable(
                name: "NavLinks",
                columns: table => new
                {
                    NavLinkId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    PageId = table.Column<int>(nullable: false),
                    ParentLinkId = table.Column<int>(nullable: false),
                    Position = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavLinks", x => x.NavLinkId);
                    table.ForeignKey(
                        name: "FK_NavLinks_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "PageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NavLinks_Pages_ParentLinkId",
                        column: x => x.ParentLinkId,
                        principalTable: "Pages",
                        principalColumn: "PageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedPages",
                columns: table => new
                {
                    RelatedPagesId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Page1Id = table.Column<int>(nullable: false),
                    Page2Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedPages", x => x.RelatedPagesId);
                    table.ForeignKey(
                        name: "FK_RelatedPages_Pages_Page1Id",
                        column: x => x.Page1Id,
                        principalTable: "Pages",
                        principalColumn: "PageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedPages_Pages_Page2Id",
                        column: x => x.Page2Id,
                        principalTable: "Pages",
                        principalColumn: "PageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavLinks_PageId",
                table: "NavLinks",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_NavLinks_ParentLinkId",
                table: "NavLinks",
                column: "ParentLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedPages_Page1Id",
                table: "RelatedPages",
                column: "Page1Id");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedPages_Page2Id",
                table: "RelatedPages",
                column: "Page2Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavLinks");

            migrationBuilder.DropTable(
                name: "RelatedPages");

            migrationBuilder.DropTable(
                name: "Pages");
        }
    }
}
