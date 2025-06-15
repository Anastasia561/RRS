using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RRS.Models;
using Version = RRS.Models.Version;

namespace RRS.Data;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Individual> Individuals { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Software> Softwares { get; set; }

    public virtual DbSet<Update> Updates { get; set; }

    public virtual DbSet<Version> Versions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("s30200");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pk");

            entity.ToTable("category");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pk");

            entity.ToTable("employee");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(10)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_role");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pk");

            entity.ToTable("role");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pk");

            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pk");

            entity.ToTable("company");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Krs)
                .HasMaxLength(20)
                .HasColumnName("krs");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Company)
                .HasForeignKey<Company>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Company_Client");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contract_pk");

            entity.ToTable("contract");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("final_price");
            entity.Property(e => e.IsSigned).HasColumnName("is_signed");
            entity.Property(e => e.SoftwareId).HasColumnName("software_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdateSupport).HasColumnName("update_support");
            entity.Property(e => e.VersionId).HasColumnName("version_id");

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contruct_client");

            entity.HasOne(d => d.Software).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SoftwareId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contruct_software");

            entity.HasOne(d => d.Version).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.VersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_version");

            entity.HasMany(d => d.Updates).WithMany(p => p.Contracts)
                .UsingEntity<Dictionary<string, object>>(
                    "ContructUpdate",
                    r => r.HasOne<Update>().WithMany()
                        .HasForeignKey("UpdateId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("contruct_update_update"),
                    l => l.HasOne<Contract>().WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("contruct_update_contruct"),
                    j =>
                    {
                        j.HasKey("ContractId", "UpdateId").HasName("contruct_update_pk");
                        j.ToTable("contruct_update");
                        j.IndexerProperty<int>("ContractId").HasColumnName("contract_id");
                        j.IndexerProperty<int>("UpdateId").HasColumnName("update_id");
                    });
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discount_pk");

            entity.ToTable("discount");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsUpfront).HasColumnName("is_upfront");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasMany(d => d.Softwares).WithMany(p => p.Discounts)
                .UsingEntity<Dictionary<string, object>>(
                    "SoftwareDiscount",
                    r => r.HasOne<Software>().WithMany()
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("software_discount_software"),
                    l => l.HasOne<Discount>().WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("software_discount_discount"),
                    j =>
                    {
                        j.HasKey("DiscountId", "SoftwareId").HasName("software_discount_pk");
                        j.ToTable("software_discount");
                        j.IndexerProperty<int>("DiscountId").HasColumnName("discount_id");
                        j.IndexerProperty<int>("SoftwareId").HasColumnName("software_id");
                    });
        });

        modelBuilder.Entity<Individual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("individual_pk");

            entity.ToTable("individual");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .HasColumnName("first_name");
            entity.Property(e => e.IsRemoved).HasColumnName("is_removed");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasColumnName("last_name");
            entity.Property(e => e.Pesel)
                .HasMaxLength(50)
                .HasColumnName("pesel");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Individual)
                .HasForeignKey<Individual>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Individual_Client");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_pk");

            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payment_contract");
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("software_pk");

            entity.ToTable("software");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CurrentVersionId).HasColumnName("current_version_id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.YearCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("year_cost");

            entity.HasOne(d => d.Category).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Software_Category");

            entity.HasOne(d => d.CurrentVersion).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.CurrentVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("software_version");
        });

        modelBuilder.Entity<Update>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("update_pk");

            entity.ToTable("update");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("version_pk");

            entity.ToTable("version");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("release_date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}