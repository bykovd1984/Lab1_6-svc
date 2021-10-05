﻿// <auto-generated />
using System;
using Lab1_6.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lab1_6.Data.Migrations.Migrations
{
    [DbContext(typeof(UsersDbContext))]
    [Migration("20210923041948_AddTimestamp")]
    partial class AddTimestamp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Lab1_6.Data.Profile", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.HasKey("UserName");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Lab1_6.Models.Billing.Account", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<int>("Deposit")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("UserName");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Lab1_6.Models.Orders.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Sum")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Lab1_6.Models.Orders.OrderRequest", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<string>("RequestId")
                        .HasColumnType("text");

                    b.HasKey("OrderId");

                    b.HasIndex("RequestId")
                        .IsUnique();

                    b.ToTable("OrderRequests");
                });

            modelBuilder.Entity("Lab1_6.Models.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Lab1_6.Models.Orders.OrderRequest", b =>
                {
                    b.HasOne("Lab1_6.Models.Orders.Order", "Order")
                        .WithOne()
                        .HasForeignKey("Lab1_6.Models.Orders.OrderRequest", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });
#pragma warning restore 612, 618
        }
    }
}
