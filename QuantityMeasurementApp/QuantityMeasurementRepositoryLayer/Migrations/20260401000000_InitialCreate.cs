using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuantityMeasurementRepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuantityMeasurements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MeasurementId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    FirstOperandValue = table.Column<double>(type: "double precision", nullable: true),
                    FirstOperandUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FirstOperandCategory = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondOperandValue = table.Column<double>(type: "double precision", nullable: true),
                    SecondOperandUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondOperandCategory = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TargetUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SourceOperandValue = table.Column<double>(type: "double precision", nullable: true),
                    SourceOperandUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SourceOperandCategory = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ResultValue = table.Column<double>(type: "double precision", nullable: true),
                    ResultUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FormattedResult = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorDetails = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Indexes - QuantityMeasurements
            migrationBuilder.CreateIndex(name: "IX_QuantityMeasurements_MeasurementId", table: "QuantityMeasurements", column: "MeasurementId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_QuantityMeasurements_CreatedAt", table: "QuantityMeasurements", column: "CreatedAt");
            migrationBuilder.CreateIndex(name: "IX_QuantityMeasurements_OperationType", table: "QuantityMeasurements", column: "OperationType");
            migrationBuilder.CreateIndex(name: "IX_QuantityMeasurements_FirstOperandCategory", table: "QuantityMeasurements", column: "FirstOperandCategory");
            migrationBuilder.CreateIndex(name: "IX_QuantityMeasurements_IsSuccessful", table: "QuantityMeasurements", column: "IsSuccessful");

            // Indexes - Users
            migrationBuilder.CreateIndex(name: "IX_Users_Username", table: "Users", column: "Username", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);

            // Indexes - RefreshTokens
            migrationBuilder.CreateIndex(name: "IX_RefreshTokens_Token", table: "RefreshTokens", column: "Token", unique: true);
            migrationBuilder.CreateIndex(name: "IX_RefreshTokens_UserId", table: "RefreshTokens", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_RefreshTokens_ExpiresAt", table: "RefreshTokens", column: "ExpiresAt");
            migrationBuilder.CreateIndex(name: "IX_RefreshTokens_RevokedAt", table: "RefreshTokens", column: "RevokedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RefreshTokens");
            migrationBuilder.DropTable(name: "QuantityMeasurements");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
