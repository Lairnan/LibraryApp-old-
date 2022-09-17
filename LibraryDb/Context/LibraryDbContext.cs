using EncryptionSample;
using LibraryDb.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Type = LibraryDb.Items.Type;

namespace LibraryDb.Context
{
    
    public partial class LibraryDbContext : DbContext
    {
        private static string SecretHash { get; set; }

        public LibraryDbContext()
        {
        }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public static void SetHash(string secretHash)
        {
            SecretHash = secretHash;
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Loan> Loans { get; set; } = null!;
        public virtual DbSet<Reader> Readers { get; set; } = null!;
        public virtual DbSet<Style> Styles { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            var key = Registry.CurrentUser.OpenSubKey("LibraryAPP", false);
            var host = key.GetValue("host").ToString();
            var user = key.GetValue("user").ToString();
            var pass = key.GetValue("password").ToString();
            var db = key.GetValue("database").ToString();
            var port = key.GetValue("port").ToString();
                
            optionsBuilder.UseNpgsql($"host={host.Decrypt(SecretHash)};" +
                                     $"port={port};" +
                                     $"user id={user.Decrypt(SecretHash)};" +
                                     $"password={pass.Decrypt(SecretHash)};" +
                                     $"database={db.Decrypt(SecretHash)}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("authors");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(50)
                    .HasColumnName("patronymic");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .HasColumnName("surname");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuthorId).HasColumnName("author_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.StyleId).HasColumnName("style_id");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("books_author_id_fkey");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("books_categorie_id_fkey");

                entity.HasOne(d => d.Style)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.StyleId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("books_style_id_fkey");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("loans");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.Passed).HasColumnName("passed");

                entity.Property(e => e.ReaderId).HasColumnName("reader_id");

                entity.Property(e => e.TakenDate).HasColumnName("taken_date");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("loans_book_id_fkey");

                entity.HasOne(d => d.Reader)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.ReaderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("loans_reader_id_fkey");
            });

            modelBuilder.Entity<Reader>(entity =>
            {
                entity.ToTable("readers");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('books_id_seq'::regclass)");

                entity.Property(e => e.Birthday).HasColumnName("birthday");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(30)
                    .HasColumnName("patronymic");

                entity.Property(e => e.Surname)
                    .HasMaxLength(30)
                    .HasColumnName("surname");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Readers)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("readers_type_id_fkey");
            });

            modelBuilder.Entity<Style>(entity =>
            {
                entity.ToTable("styles");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('style_id_seq'::regclass)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .HasColumnName("name");
            });

            modelBuilder.HasSequence("authors_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("books_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("categories_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("loans_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("style_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("types_id_seq").HasMax(2147483647);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
