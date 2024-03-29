﻿using ASPProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASPProject.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("asp");
            modelBuilder.Entity<Token>()
                .Property(t => t.Id)
                .HasMaxLength(32)
                .IsFixedLength();

            modelBuilder.Entity<Rate>().HasKey(nameof(Rate.ItemId), nameof(Rate.UserId));

            modelBuilder.Entity<Section>().HasOne(s=> s.Author).WithMany().HasForeignKey(s=>s.AuthorId);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Author).WithMany()
                .HasForeignKey(t => t.AuthorId);
            modelBuilder.Entity<Section>()
              .HasMany(s => s.Rates)
              .WithOne()
              .HasForeignKey(r => r.ItemId);

            modelBuilder.Entity<Theme>()
                .HasOne(t => t.Author).WithMany()
                .HasForeignKey(t => t.AuthorId);

            modelBuilder.Entity<Theme>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Theme)
                .HasForeignKey(c => c.ThemeId);

            modelBuilder.Entity<Comment>()
               .HasOne(e => e.Author).WithMany()
                .HasForeignKey(e => e.AuthorId);
        }
    }
}
