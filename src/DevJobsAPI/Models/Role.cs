namespace DevJobsAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Role
    {
        public Role()
        {
            this.Users = new List<User>();  
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 50 characters.")]
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; }
    }
}
