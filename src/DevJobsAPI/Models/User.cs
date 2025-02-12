namespace DevJobsAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        public User()
        {
            this.Applications = new List<Application>();
            this.SavedJobs = new List<SavedJob>();
            this.Jobs = new List<Job>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string PasswordHash { get; set; } = null!;

        [ForeignKey("Role")]
        public long RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Application>? Applications { get; set; }
        public ICollection<SavedJob>? SavedJobs { get; set; }
        public ICollection<Job>? Jobs { get; set; }

    }
}
