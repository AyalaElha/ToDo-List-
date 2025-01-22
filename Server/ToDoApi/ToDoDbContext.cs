using Microsoft.EntityFrameworkCore;

namespace ToDoApi
{
    public partial class ToDoDbContext : DbContext
    {
        // בנאי
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
            : base(options)
        {
        }

        // הגדרת DbSet של משימות
        public virtual DbSet<Item> Items { get; set; }

        // הגדרת המודל (למשל, שם הטבלה ויחסים)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci") // קידוד
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Item>(entity =>
            {
                // הגדרת המפתח הראשי של הטבלה
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                // הגדרת שם הטבלה במסד הנתונים
                entity.ToTable("item");

                // הגדרת הגבלה על אורך השם של המשימה
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        // פונקציה חלקית שניתן להוסיף לה הגדרות נוספות
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
