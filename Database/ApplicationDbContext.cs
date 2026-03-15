using System;
using Microsoft.EntityFrameworkCore;
using wealthify.Entity;

namespace wealthify.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<InvestmentType> InvestmentTypes { get; set; }
    public DbSet<FamilyMember> FamilyMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<InvestmentType>(entity =>
            {
                entity.ToTable("investment_types");

                entity.HasKey(it => it.Id);

                entity.Property(it => it.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(it => it.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(it => it.Name)
                    .IsUnique();

                entity.Property(it => it.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(it => it.UpdatedAt)
                    .IsRequired(false);
            }
        );

        builder.Entity<FamilyMember>(entity =>
            {
                entity.ToTable("family_members");

                entity.HasKey(it => it.Id);

                entity.Property(it => it.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(it => it.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(it => it.Name)
                    .IsUnique();

                entity.Property(it => it.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(it => it.UpdatedAt)
                    .IsRequired(false);
            }
        );
    }
}
