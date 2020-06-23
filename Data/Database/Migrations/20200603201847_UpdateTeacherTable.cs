// <copyright file="20200603201847_UpdateTeacherTable.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Updates the teacher table to use a nullable user id.
    /// </summary>
    public partial class UpdateTeacherTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_AspNetUsers_AppUserId",
                table: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Teacher_AppUserId",
                table: "Teacher");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Teacher",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_AppUserId",
                table: "Teacher",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_AspNetUsers_AppUserId",
                table: "Teacher",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_AspNetUsers_AppUserId",
                table: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Teacher_AppUserId",
                table: "Teacher");

            migrationBuilder.AlterColumn<Guid>(
                name: "AppUserId",
                table: "Teacher",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_AppUserId",
                table: "Teacher",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_AspNetUsers_AppUserId",
                table: "Teacher",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
