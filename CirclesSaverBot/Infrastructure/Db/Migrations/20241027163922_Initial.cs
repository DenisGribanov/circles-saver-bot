using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TgUser",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tg_user_pk", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "UserActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramUpdate = table.Column<string>(type: "text", nullable: false),
                    UserState = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TgMediaFile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    FileUniqueId = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    VideoDuration = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HashTags = table.Column<string[]>(type: "text[]", nullable: true),
                    OwnerTgUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsVisable = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TgMediaFile", x => x.Id);
                    table.ForeignKey(
                        name: "tg_media_file_tg_user_fk_id",
                        column: x => x.OwnerTgUserId,
                        principalTable: "TgUser",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InlineResultStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TgMediaFileId = table.Column<long>(type: "bigint", nullable: false),
                    TgUserId = table.Column<long>(type: "bigint", nullable: false),
                    ClickCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InlineResultStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "inline_result_statistics_tg_media_file_fk_id",
                        column: x => x.TgMediaFileId,
                        principalTable: "TgMediaFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "inline_result_statistics_tg_user_fk_id",
                        column: x => x.TgUserId,
                        principalTable: "TgUser",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InlineResultStatistics_TgMediaFileId",
                table: "InlineResultStatistics",
                column: "TgMediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_InlineResultStatistics_TgUserId",
                table: "InlineResultStatistics",
                column: "TgUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TgMediaFile_OwnerTgUserId",
                table: "TgMediaFile",
                column: "OwnerTgUserId");

            migrationBuilder.CreateIndex(
                name: "tg_media_file_id_index",
                table: "TgMediaFile",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "tg_user_chat_id_uindex",
                table: "TgUser",
                column: "ChatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InlineResultStatistics");

            migrationBuilder.DropTable(
                name: "UserActions");

            migrationBuilder.DropTable(
                name: "TgMediaFile");

            migrationBuilder.DropTable(
                name: "TgUser");
        }
    }
}