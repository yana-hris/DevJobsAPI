namespace DevJobsAPI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using Microsoft.EntityFrameworkCore;
    
    using DevJobsAPI.Data;
    using DevJobsAPI.Services.Interfaces;
    using DevJobsAPI.ViewModels;
    using DevJobsAPI.Models;
    using DevJobsAPI.Models.Enums;

    public class JobService : IJobService
    {
        private readonly ApplicationDbContext context;

        public JobService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<long> AddAsync(JobFormModel job)
        {
            Job newJob = new Job()
            {
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                Salary = job.Salary,
                Company = job.Company,
                MinExperience = job.MinExperience,
                MaxExperience = job.MaxExperience,
                WorkMode = job.WorkMode.HasValue ? (WorkMode)job.WorkMode.Value : WorkMode.OnSite,
                Level = job.Level.HasValue ? (Level)job.Level.Value : Level.Junior,
                JobType = Enum.IsDefined(typeof(JobType), job.JobType) ? (JobType)job.JobType : throw new ArgumentException("Invalid JobType"),
                EmployerId = job.EmployerId,
            };

            await context.Jobs.AddAsync(newJob);
            await context.SaveChangesAsync();

            return newJob.Id;
        }

       
        public async Task<ICollection<JobViewModel>> GetAllAsync()
        {
            var model = await context.Jobs.Select(j => new JobViewModel
            {
                Title = j.Title,
                Description = j.Description,
                Company = j.Company,
                Location = j.Location,
                MinExperience = j.MinExperience,
                MaxExperience = j.MaxExperience,
                WorkMode = j.WorkMode.ToString(),
                JobType = j.JobType.ToString(),
                Level = j.Level.ToString(),
                Salary = j.Salary
            })
            .ToListAsync();

            return model;
        }

        public async Task<JobViewModel?> GetByIdAsync(long id)
        {
            var model = await context.Jobs.Where(j => j.Id == id)
                .Select(j => new JobViewModel()
                {
                    Title = j.Title,
                    Description = j.Description,
                    Company = j.Company,
                    Location = j.Location,
                    MinExperience = j.MinExperience,
                    MaxExperience = j.MaxExperience,
                    WorkMode = j.WorkMode.ToString(),
                    JobType = j.JobType.ToString(),
                    Level = j.Level.ToString(),
                    Salary = j.Salary
                })
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task UpdateAsync(JobFormModel model)
        {
            Job? jobToUpdate = await context.Jobs.FindAsync(model.Id);

            if (jobToUpdate == null)
            {
                throw new ArgumentNullException("Job not found");
            }

            jobToUpdate.Title = model.Title;
            jobToUpdate.Description = model.Description;
            jobToUpdate.Location = model.Location;
            jobToUpdate.Salary = model.Salary;
            jobToUpdate.Company = model.Company;
            jobToUpdate.MinExperience = model.MinExperience;
            jobToUpdate.MaxExperience = model.MaxExperience;
            jobToUpdate.WorkMode = model.WorkMode.HasValue ? (WorkMode)model.WorkMode.Value : WorkMode.OnSite;
            jobToUpdate.Level = model.Level.HasValue ? (Level)model.Level.Value : Level.Junior;
            jobToUpdate.JobType = Enum.IsDefined(typeof(JobType), model.JobType) ? (JobType)model.JobType : throw new ArgumentException("Invalid JobType");
            jobToUpdate.EmployerId = model.EmployerId;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            Job? jobToDelete = await context.Jobs
                .Where(j => j.Id == id)
                .FirstOrDefaultAsync();

            if (jobToDelete == null)
            {
                throw new ArgumentNullException("Job not found.");
            }

            context.Jobs.Remove(jobToDelete);
            await context.SaveChangesAsync();
        }

    }
}
