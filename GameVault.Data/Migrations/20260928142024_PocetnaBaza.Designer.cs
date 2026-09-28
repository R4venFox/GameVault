using System;
using GameVault.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameVault.Data.Migrations
{
    [DbContext(typeof(GameVaultDbContext))]
    [Migration("20260928142024_PocetnaBaza")]
    partial class PocetnaBaza
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.20");

            modelBuilder.Entity("GameVault.Data.Models.Igra", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BrojSati")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DatumDodavanja")
                        .HasColumnType("TEXT");

                    b.Property<string>("Developer")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GodinaIzdanja")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Izdavac")
                        .HasColumnType("TEXT");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Ocena")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Omiljena")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Opis")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Igre", null, t =>
                        {
                            t.HasCheckConstraint("CK_Igre_BrojSati", "BrojSati >= 0");

                            t.HasCheckConstraint("CK_Igre_Naziv", "length(trim(Naziv)) > 0");

                            t.HasCheckConstraint("CK_Igre_Ocena", "Ocena IS NULL OR Ocena BETWEEN 1 AND 10");

                            t.HasCheckConstraint("CK_Igre_Status", "Status IN (0, 1, 2, 3)");
                        });
                });

            modelBuilder.Entity("GameVault.Data.Models.Platforma", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Naziv")
                        .IsUnique();

                    b.ToTable("Platforme", null, t =>
                        {
                            t.HasCheckConstraint("CK_Platforme_Naziv", "length(trim(Naziv)) > 0");
                        });
                });

            modelBuilder.Entity("GameVault.Data.Models.Zanr", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Naziv")
                        .IsUnique();

                    b.ToTable("Zanrovi", null, t =>
                        {
                            t.HasCheckConstraint("CK_Zanrovi_Naziv", "length(trim(Naziv)) > 0");
                        });
                });

            modelBuilder.Entity("IgraPlatforma", b =>
                {
                    b.Property<int>("IgreId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlatformeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("IgreId", "PlatformeId");

                    b.HasIndex("PlatformeId");

                    b.ToTable("IgraPlatforma");
                });

            modelBuilder.Entity("IgraZanr", b =>
                {
                    b.Property<int>("IgreId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ZanroviId")
                        .HasColumnType("INTEGER");

                    b.HasKey("IgreId", "ZanroviId");

                    b.HasIndex("ZanroviId");

                    b.ToTable("IgraZanr");
                });

            modelBuilder.Entity("IgraPlatforma", b =>
                {
                    b.HasOne("GameVault.Data.Models.Igra", null)
                        .WithMany()
                        .HasForeignKey("IgreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameVault.Data.Models.Platforma", null)
                        .WithMany()
                        .HasForeignKey("PlatformeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IgraZanr", b =>
                {
                    b.HasOne("GameVault.Data.Models.Igra", null)
                        .WithMany()
                        .HasForeignKey("IgreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameVault.Data.Models.Zanr", null)
                        .WithMany()
                        .HasForeignKey("ZanroviId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
