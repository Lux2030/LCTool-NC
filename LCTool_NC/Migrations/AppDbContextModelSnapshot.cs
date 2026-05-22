using System;
using LCTool_NC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LCTool_NC.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LCTool_NC.Models.NonConformita", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AzioniCorrettive")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Causa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cliente")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("CreatoDa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("DataApertura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataEffettivaChiusura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataPrevistaChiusura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataRevisione")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descrizione")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DipartimentoCoinvolto")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("DocSgqModificati")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("IsoNormaRiferimento")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ItemRiferimento")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ModificatoDa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NoteRevisione")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Numero")
                        .HasColumnType("int");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PartiInteressate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RevisionatoDa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RilevataDa")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Risoluzione")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sedi")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Stato")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Aperta");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");
                    b.HasIndex("Numero").IsUnique();
                    b.ToTable("NonConformita");
                });

            modelBuilder.Entity("LCTool_NC.Models.NonConformitaPartiInteressate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("FlagAttivo").HasColumnType("bit");
                    b.Property<int>("Ordine").HasColumnType("int");
                    b.Property<string>("Valore")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");
                    b.ToTable("NonConformitaPartiInteressate");
                });

            modelBuilder.Entity("LCTool_NC.Models.NonConformitaRilevataDa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("FlagAttivo").HasColumnType("bit");
                    b.Property<int>("Ordine").HasColumnType("int");
                    b.Property<string>("Valore")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");
                    b.ToTable("NonConformitaRilevataDa");
                });

            modelBuilder.Entity("LCTool_NC.Models.NonConformitaStati", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("FlagAttivo").HasColumnType("bit");
                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");
                    b.ToTable("NonConformitaStati");
                });
#pragma warning restore 612, 618
        }
    }
}
