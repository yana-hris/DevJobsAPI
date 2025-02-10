namespace DevJobsAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Application
    {
        public Application()
        {
            this.AppliedAt = DateTime.UtcNow;
        }

        [Key, Column(Order = 1)]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        [Key, Column(Order = 2)]
        [ForeignKey("Job")]
        public long JobId { get; set; }
        public Job Job { get; set; } = null!;

        public string Status { get; set; } = null!; // Pending, Accepted, Rejected
        public DateTime AppliedAt { get; set; }
    }
}
