namespace DevJobsAPI.Data.Configurations
{
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using DevJobsAPI.Models;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Define Relationship with Role
            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Seed Default Roles
            builder.HasData(
                new User
                {
                    Id = 1,
                    FullName = "Alice Johnson",
                    Email = "alice@example.com",
                    PasswordHash = HashPassword("password123"),
                    RoleId = 1 // Employer
                },
            new User
            {
                Id = 2,
                FullName = "Bob Smith",
                Email = "bob@example.com",
                PasswordHash = HashPassword("password456"),
                RoleId = 2 // Employee
            },
            new User
            {
                Id = 3,
                FullName = "Charlie Admin",
                Email = "charlie@example.com",
                PasswordHash = HashPassword("adminpass"),
                RoleId = 3 // Admin
            }
            );
        }

        // Helper function to hash passwords before storing
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
