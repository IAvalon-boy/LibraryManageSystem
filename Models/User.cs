using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; } = string.Empty; // University ID

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Faculty { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string UserType { get; set; } = string.Empty; // Student, Teacher, Admin

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}