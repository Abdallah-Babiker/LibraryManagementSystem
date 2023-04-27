using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryManagementSystem.Models
{
    public partial class Entities : DbContext
    {
        public Entities()
        {
        }

        public Entities(DbContextOptions<Entities> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<Rack> Rack { get; set; }
        public virtual DbSet<Shelf> Shelf { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=.;Database=LibraryMgtDB;Trusted_Connection=True;", builder => builder.UseRowNumberForPaging());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Author).HasMaxLength(50);

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 1)");

                entity.HasOne(d => d.Shelf)
                    .WithMany(p => p.Book)
                    .HasForeignKey(d => d.ShelfId)
                    .HasConstraintName("FK_Books_Shelves");
            });

            modelBuilder.Entity<Rack>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(20);
            });

            modelBuilder.Entity<Shelf>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(20);

                entity.HasOne(d => d.Rack)
                    .WithMany(p => p.Shelf)
                    .HasForeignKey(d => d.RackId)
                    .HasConstraintName("FK_Shelves_Racks");
            });
        }
    }
}
