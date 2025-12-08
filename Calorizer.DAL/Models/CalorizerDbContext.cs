using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Calorizer.DAL.Models;

public partial class CalorizerDbContext : DbContext
{
    public CalorizerDbContext()
    {
    }

    public CalorizerDbContext(DbContextOptions<CalorizerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BiochemicalMedicalTest> BiochemicalMedicalTests { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<DrugsSupplement> DrugsSupplements { get; set; }

    public virtual DbSet<Lookup> Lookups { get; set; }

    public virtual DbSet<LookupCategory> LookupCategories { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<WeightHistory> WeightHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KHALIDSALAMEH;Database=Calorizer;Integrated Security=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BiochemicalMedicalTest>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.BiochemicalMedicalTests)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BiochemicalMedicalTests_Clients");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(2000);
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.FullNameAr).HasMaxLength(500);
            entity.Property(e => e.FullNameEn).HasMaxLength(500);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Gender).WithMany(p => p.Clients)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Lookups");
        });

        modelBuilder.Entity<DrugsSupplement>(entity =>
        {
            entity.ToTable("DrugsSupplement");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.DrugsSupplements)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DrugsSupplement_Clients");
        });

        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.NameAr).HasMaxLength(200);
            entity.Property(e => e.NameEn).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Lookups)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lookups_LookupCategories");
        });

        modelBuilder.Entity<LookupCategory>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.NameAr).HasMaxLength(200);
            entity.Property(e => e.NameEn).HasMaxLength(200);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.MedicalHistories)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicalHistories_Clients");
        });

        modelBuilder.Entity<WeightHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WeightClientsHistories");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Client).WithMany(p => p.WeightHistories)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WeightClientsHistories_Clients");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
