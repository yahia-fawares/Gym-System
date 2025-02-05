﻿// <auto-generated />
using System;
using GymSystem2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace GymSystem2.Migrations
{
    [DbContext(typeof(ModelContext))]
    [Migration("20250114180038_AddIsApproveColumnToTestimonial")]
    partial class AddIsApproveColumnToTestimonial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("C##GYM")
                .UseCollation("USING_NLS_COMP")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GymSystem2.Models.Card", b =>
                {
                    b.Property<decimal>("Cardid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("CARDID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Cardid"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("NUMBER")
                        .HasColumnName("BALANCE");

                    b.Property<decimal>("Password")
                        .HasColumnType("NUMBER")
                        .HasColumnName("PASSWORD");

                    b.HasKey("Cardid")
                        .HasName("SYS_C008625");

                    b.ToTable("CARD", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Plan", b =>
                {
                    b.Property<decimal>("Planid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("PLANID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Planid"));

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("DESCRIPTION");

                    b.Property<string>("Planname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("PLANNAME");

                    b.Property<decimal>("Price")
                        .HasColumnType("NUMBER(10,2)")
                        .HasColumnName("PRICE");

                    b.Property<decimal?>("Trainerid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("TRAINERID");

                    b.HasKey("Planid")
                        .HasName("SYS_C008641");

                    b.HasIndex("Trainerid");

                    b.ToTable("PLAN", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Profile", b =>
                {
                    b.Property<decimal>("Profileid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("PROFILEID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Profileid"));

                    b.Property<string>("Fname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("FNAME");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(10)")
                        .HasColumnName("GENDER");

                    b.Property<string>("Imagepath")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("IMAGEPATH");

                    b.Property<string>("Lname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("LNAME");

                    b.HasKey("Profileid")
                        .HasName("SYS_C008629");

                    b.ToTable("PROFILE", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Role", b =>
                {
                    b.Property<decimal>("Roleid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("ROLEID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Roleid"));

                    b.Property<string>("Rolename")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("ROLENAME");

                    b.HasKey("Roleid")
                        .HasName("SYS_C008621");

                    b.ToTable("ROLE", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Subscription", b =>
                {
                    b.Property<decimal>("Subscriptionid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("SUBSCRIPTIONID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Subscriptionid"));

                    b.Property<DateTime>("Fromdate")
                        .HasColumnType("DATE")
                        .HasColumnName("FROMDATE");

                    b.Property<decimal?>("Memberid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("MEMBERID");

                    b.Property<decimal?>("Planid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("PLANID");

                    b.Property<DateTime>("Todate")
                        .HasColumnType("DATE")
                        .HasColumnName("TODATE");

                    b.HasKey("Subscriptionid")
                        .HasName("SYS_C008646");

                    b.HasIndex("Memberid");

                    b.HasIndex("Planid");

                    b.ToTable("SUBSCRIPTION", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Testimonial", b =>
                {
                    b.Property<decimal>("Testimonialid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("TESTIMONIALID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Testimonialid"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("CONTENT");

                    b.Property<bool>("IsApprove")
                        .HasColumnType("NUMBER(1)");

                    b.Property<decimal?>("Userid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("USERID");

                    b.HasKey("Testimonialid")
                        .HasName("SYS_C008651");

                    b.HasIndex("Userid");

                    b.ToTable("TESTIMONIAL", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.User", b =>
                {
                    b.Property<decimal>("Userid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER")
                        .HasColumnName("USERID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Userid"));

                    b.Property<decimal?>("Cardid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("CARDID");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("EMAIL");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(255)")
                        .HasColumnName("PASSWORD");

                    b.Property<decimal?>("Profileid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("PROFILEID");

                    b.Property<decimal?>("Roleid")
                        .HasColumnType("NUMBER")
                        .HasColumnName("ROLEID");

                    b.HasKey("Userid")
                        .HasName("SYS_C008633");

                    b.HasIndex("Cardid");

                    b.HasIndex("Profileid");

                    b.HasIndex("Roleid");

                    b.HasIndex(new[] { "Email" }, "SYS_C008634")
                        .IsUnique();

                    b.ToTable("USERS", "C##GYM");
                });

            modelBuilder.Entity("GymSystem2.Models.Plan", b =>
                {
                    b.HasOne("GymSystem2.Models.User", "Trainer")
                        .WithMany("Plans")
                        .HasForeignKey("Trainerid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_PLAN_TRAINER");

                    b.Navigation("Trainer");
                });

            modelBuilder.Entity("GymSystem2.Models.Subscription", b =>
                {
                    b.HasOne("GymSystem2.Models.User", "Member")
                        .WithMany("Subscriptions")
                        .HasForeignKey("Memberid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SUBSCRIPTION_MEMBER");

                    b.HasOne("GymSystem2.Models.Plan", "Plan")
                        .WithMany("Subscriptions")
                        .HasForeignKey("Planid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_SUBSCRIPTION_PLAN");

                    b.Navigation("Member");

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("GymSystem2.Models.Testimonial", b =>
                {
                    b.HasOne("GymSystem2.Models.User", "User")
                        .WithMany("Testimonials")
                        .HasForeignKey("Userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_TESTIMONIAL_USER");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GymSystem2.Models.User", b =>
                {
                    b.HasOne("GymSystem2.Models.Card", "Card")
                        .WithMany("Users")
                        .HasForeignKey("Cardid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_USER_CARD");

                    b.HasOne("GymSystem2.Models.Profile", "Profile")
                        .WithMany("Users")
                        .HasForeignKey("Profileid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_USER_PROFILE");

                    b.HasOne("GymSystem2.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("Roleid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_USER_ROLE");

                    b.Navigation("Card");

                    b.Navigation("Profile");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("GymSystem2.Models.Card", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GymSystem2.Models.Plan", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("GymSystem2.Models.Profile", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GymSystem2.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GymSystem2.Models.User", b =>
                {
                    b.Navigation("Plans");

                    b.Navigation("Subscriptions");

                    b.Navigation("Testimonials");
                });
#pragma warning restore 612, 618
        }
    }
}
