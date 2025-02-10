namespace DevJobsAPI.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using DevJobsAPI.Models;

    public class SavedJobConfiguration : IEntityTypeConfiguration<SavedJob>
    {
        public void Configure(EntityTypeBuilder<SavedJob> builder)
        {
            // Composite Primary Key (UserId + JobId)
            builder.HasKey(saved => new { saved.UserId, saved.JobId });

            // Define Relationships
            builder.HasOne(saved => saved.User)
                   .WithMany(user => user.SavedJobs)
                   .HasForeignKey(saved => saved.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(saved => saved.Job)
                   .WithMany(job => job.SavedJobs)
                   .HasForeignKey(saved => saved.JobId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
