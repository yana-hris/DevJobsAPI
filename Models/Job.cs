namespace DevJobsAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using DevJobsAPI.Models.Enums;

    public class Job
    {
        public Job()
        {
            this.Applications = new List<Application>();
            this.SavedJobs = new List<SavedJob>();
            this.SavedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(50, ErrorMessage = "Description must be at least 50 characters long.")]
        public string Description { get; set; } = null!;
        
        public DateTime SavedAt { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string Company { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; } = null!;

        [Range(0, 50, ErrorMessage = "Minimum experience must be between 0 and 50 years.")]
        public int MinExperience { get; set; }

        [Range(0, 50, ErrorMessage = "Maximum experience must be between 0 and 50 years.")]
        public int MaxExperience { get; set; }

        [NotMapped] // This is not stored in DB but is used for validation
        public bool IsValidExperienceRange => MinExperience <= MaxExperience;

        [Required]
        [EnumDataType(typeof(WorkMode))]
        public WorkMode WorkMode { get; set; } 

        [Required]
        [EnumDataType(typeof(JobType))]
        public JobType JobType { get; set; }

        [Required]
        [EnumDataType(typeof(Level))]
        [Display(Name = "Experience Level")]
        public Level Level { get; set; } 

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Salary { get; set; }

        [ForeignKey("Employer")]
        [Required]
        public long EmployerId { get; set; }
        public User Employer { get; set; } = null!; 

        public ICollection<Application>? Applications { get; set; }
        public ICollection<SavedJob>? SavedJobs { get; set; }
    }
}
