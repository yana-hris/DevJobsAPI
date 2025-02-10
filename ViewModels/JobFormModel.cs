namespace DevJobsAPI.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class JobFormModel
    {
        [Required]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(50, ErrorMessage = "Description must be at least 50 characters long.")]
        public string Description { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string Company { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; } = null!;

        [Range(0, 50, ErrorMessage = "Minimum experience must be between 0 and 50 years.")]
        public int MinExperience { get; set; }

        [Range(0, 50, ErrorMessage = "Maximum experience must be between 0 and 50 years.")]
        public int MaxExperience { get; set; }
       
        [Range(1, 3)]
        public int? WorkMode { get; set; }  // Enum

        
        [Required]
        [Range(1, 6)]
        public int JobType { get; set; }  // Enum

        
        [Range(1, 3)]
        public int? Level { get; set; }  // Enum

        [Range(0, double.MaxValue)]
        [Required]
        public decimal Salary { get; set; }

        [Required]
        [Range(0, long.MaxValue)]
        public long EmployerId { get; set; }
    }
}
