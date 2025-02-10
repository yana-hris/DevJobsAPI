namespace DevJobsAPI.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using DevJobsAPI.Models;

    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            // Composite Primary Key (UserId + JobId)
            builder.HasKey(appl => new { appl.UserId, appl.JobId });

            // Define Relationships
            builder.HasOne(appl => appl.User)
                   .WithMany(user => user.Applications)
                   .HasForeignKey(appl => appl.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(appl => appl.Job)
                   .WithMany(job => job.Applications)
                   .HasForeignKey(appl => appl.JobId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
