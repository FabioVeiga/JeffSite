﻿// <auto-generated />
using JeffSite.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JeffSite.Migrations
{
    [DbContext(typeof(JeffContext))]
    [Migration("20210102220202_inclusao coluna de ID na tabela  social midia")]
    partial class inclusaocolunadeIDnatabelasocialmidia
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("JeffSite.Models.Configuracao", b =>
                {
                    b.Property<int>("Cod")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContactEmail")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ImgLogo")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ImgProfile")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UrlMercadoLivre")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Cod");

                    b.ToTable("Configuracao");
                });

            modelBuilder.Entity("JeffSite.Models.SocialMidia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("IconFA")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Url")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("SocialMidia");
                });

            modelBuilder.Entity("JeffSite.Models.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserName");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
