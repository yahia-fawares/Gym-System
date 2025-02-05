using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem2.Migrations
{
    public partial class AddIsApproveColumnToTestimonial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                schema: "C##GYM",
                table: "TESTIMONIAL",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprove",
                schema: "C##GYM",
                table: "TESTIMONIAL");
        }
    }
}