using Service_Login.Models;
using Microsoft.EntityFrameworkCore;

namespace Service_Login.Context
{
    public partial class UserDbContext : DbContext
    {
        public UserDbContext()
        {
        }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        // Add DbSet for User model
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply global naming convention for lowercase table and column names
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Set table name to lowercase
                entityType.SetTableName(entityType.GetTableName().ToLower());

                foreach (var property in entityType.GetProperties())
                {
                    // Set column names to lowercase
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }

            // Configure the User table to use the "users" schema and lowercase table name
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.id).HasName("users_pkey");

                entity.ToTable("users", "users"); // Ensure both table and schema names are lowercase

                // Explicitly set the column names to lowercase to match PostgreSQL's expectations
                entity.Property(e => e.id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.gitId)
                    .HasColumnName("git_id"); // Ensure the column name is in lowercase

                entity.Property(e => e.username)
                    .HasColumnName("username"); // Ensure the column name is in lowercase
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
