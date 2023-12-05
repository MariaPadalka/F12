using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ThinkTwice_Context
{

    public partial class ThinkTwiceContext : DbContext
    {
        public ThinkTwiceContext()
        {
        }

        public ThinkTwiceContext(DbContextOptions<ThinkTwiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=ThinkTwice;Trusted_Connection=True;Encrypt=False;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC074133B447");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.PercentageAmount).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Title).HasMaxLength(50);
                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.User).WithMany(p => p.Categories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Categorie__UserI__3B75D760");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC079F1B097B");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Date).HasColumnType("date");
                entity.Property(e => e.Details).HasMaxLength(255);
                entity.Property(e => e.FromCategory).HasColumnName("From_category");
                entity.Property(e => e.ToCategory).HasColumnName("To_category");

                entity.HasOne(d => d.FromCategoryNavigation).WithMany(p => p.TransactionFromCategoryNavigations)
                    .HasForeignKey(d => d.FromCategory)
                    .HasConstraintName("FK__Transacti__From___412EB0B6");

                entity.HasOne(d => d.ToCategoryNavigation).WithMany(p => p.TransactionToCategoryNavigations)
                    .HasForeignKey(d => d.ToCategory)
                    .HasConstraintName("FK__Transacti__To_ca__4222D4EF");

                entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__UserI__403A8C7D");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C01A6200");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.BirthDate).HasColumnType("date");
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(200);
                entity.Property(e => e.Surname).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}