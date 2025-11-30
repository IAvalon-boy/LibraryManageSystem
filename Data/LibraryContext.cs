using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar relaciones
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId);

            // Datos iniciales
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    ISBN = "978-0134685991",
                    Title = "Effective C#",
                    Author = "Bill Wagner",
                    Publisher = "Addison-Wesley",
                    PublicationYear = 2018,
                    Category = "Programming",
                    TotalCopies = 5,
                    AvailableCopies = 5
                },
                new Book
                {
                    Id = 2,
                    ISBN = "978-1617294532",
                    Title = "ASP.NET Core in Action",
                    Author = "Andrew Lock",
                    Publisher = "Manning",
                    PublicationYear = 2021,
                    Category = "Web Development",
                    TotalCopies = 3,
                    AvailableCopies = 3
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserId = "2023001",
                    Name = "María González",
                    Email = "maria.gonzalez@university.edu",
                    Faculty = "Engineering",
                    UserType = "Student"
                },
                new User
                {
                    Id = 2,
                    UserId = "2023002",
                    Name = "Carlos Mendoza",
                    Email = "carlos.mendoza@university.edu",
                    Faculty = "Computer Science",
                    UserType = "Student"
                }
            );
        }
    }
}