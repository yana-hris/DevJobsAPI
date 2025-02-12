namespace DevJobsAPI.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using DevJobsAPI.Models;

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Ensure Role Name is Unique
            builder.HasIndex(r => r.Name).IsUnique();

            // Seed Default Roles
            builder.HasData(
                new Role { Id = 1, Name = "Employer" },
                new Role { Id = 2, Name = "Employee" },
                new Role { Id = 3, Name = "Admin" }
            );
        }
    }
}
