namespace DevJobsAPI.Tests.UnitTests.Services
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using DevJobsAPI.Data;
    using DevJobsAPI.Models;
    using DevJobsAPI.Services;
    using DevJobsAPI.Services.Interfaces;
    using DevJobsAPI.Models.Enums;
    using DevJobsAPI.ViewModels;

    public class JobServiceTests : IDisposable
    {
        private readonly ApplicationDbContext context;
        private readonly IJobService jobService;

        public JobServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            context = new ApplicationDbContext(options);
            jobService = new JobService(context);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Jobs()
        {
            // Arrange
            context.Jobs.AddRange(new List<Job>
            {
                new Job { Title = "Backend Developer",  Description = "Develop and maintain sustainable backend systems and relational databases.", Company = "Company A", Location = "London", Salary = 70000, MinExperience = 2, JobType = JobType.FullTime, Level = Level.Mid },
                new Job { Title = "Frontend Developer", Description = "Develop and maintain existing dynamic and interactive front-ends.", Company = "Company B", Location = "Berlin", Salary = 75000, MinExperience = 3, JobType = JobType.Contract, Level = Level.Mid }
            });
            await context.SaveChangesAsync();

            // Act
            var jobs = await jobService.GetAllAsync();

            // Assert
            Assert.NotNull(jobs);
            Assert.Equal(2, jobs.Count);
            
        }

        [Fact]
        public async Task GetAllAsync_Returns_Correct_Type()
        {
            // Arrange
            context.Jobs.AddRange(new List<Job>
            {
                new Job { Title = "Backend Developer", Description = "Develop and maintain sustainable backend systems and relational databases.", Company = "Company A", Location = "London", Salary = 70000, MinExperience = 2, JobType = JobType.FullTime, Level = Level.Mid },
                new Job { Title = "Frontend Developer", Description = "Develop and maintain existing dynamic and interactive front-ends.", Company = "Company B", Location = "Berlin", Salary = 75000, MinExperience = 3, JobType = JobType.Contract, Level = Level.Mid }
            });
            await context.SaveChangesAsync();

            // Act
            var jobs = await jobService.GetAllAsync();

            // Assert
            Assert.IsAssignableFrom<ICollection<JobViewModel>>(jobs);

            // Assert that all items in the collection are of type JobViewModel
            foreach (var job in jobs)
            {
                Assert.IsType<JobViewModel>(job);  // Ensure each item is JobViewModel
            }

        }

        [Fact]
        public async Task GetAllAsync_Returns_Correct_Data()
        {
            // Arrange
            context.Jobs.AddRange(new List<Job>
            {
                new Job { Title = "Backend Developer", Description = "Develop and maintain sustainable backend systems and relational databases.", Company = "Company A", Location = "London", Salary = 70000, MinExperience = 2, MaxExperience = 5, JobType = JobType.FullTime, Level = Level.Mid },
                new Job { Title = "Frontend Developer", Description = "Develop and maintain existing dynamic and interactive front-ends.", Company = "Company B", Location = "Berlin", Salary = 155000, MinExperience = 5, JobType = JobType.Contract, Level = Level.Senior, WorkMode = WorkMode.OnSite }
            });
            await context.SaveChangesAsync();

            // Act
            var jobs = await jobService.GetAllAsync();
            
            // Assert specific values for the first item
            var firstJob = jobs.FirstOrDefault();
            Assert.NotNull(firstJob);
            Assert.Equal("Backend Developer", firstJob.Title);
            Assert.Equal("Company A", firstJob.Company);
            Assert.Equal("London", firstJob.Location);
            Assert.Equal(70000, firstJob.Salary);
            Assert.Equal(2, firstJob.MinExperience);
            Assert.Equal(5, firstJob.MaxExperience);
            Assert.Equal("FullTime", firstJob.JobType.ToString());
            Assert.Equal("Mid", firstJob.Level!.ToString());

            var secondJob = jobs.ElementAt(1);
            Assert.NotNull(secondJob);
            Assert.Equal("Frontend Developer", secondJob.Title);
            Assert.Equal("Company B", secondJob.Company);
            Assert.Equal("Berlin", secondJob.Location);
            Assert.Equal(155000, secondJob.Salary);
            Assert.Equal(5, secondJob.MinExperience);
            Assert.Equal("Contract", secondJob.JobType.ToString());
            Assert.Equal("Senior", secondJob.Level!.ToString());
            Assert.Equal("OnSite", secondJob.WorkMode!.ToString());
        }


        [Fact]
        public async Task AddAsync_Should_Add_New_Job_And_Return_Id()
        {
            // Arrange
            var jobForm = new JobFormModel
            {
                Title = "Software Engineer",
                Description = "Develop and maintain applications",
                Company = "Tech Corp",
                Location = "New York",
                Salary = 80000,
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Remote,
                Level = (int)Level.Mid,
                JobType = (int)JobType.FullTime,
                EmployerId = 1
            };

            // Act
            var jobId = await jobService.AddAsync(jobForm);
            var job = await context.Jobs.FindAsync(jobId);

            // Assert
            Assert.NotNull(job);
            Assert.IsType<long>(jobId);
            Assert.Equal(1, jobId);
            Assert.Equal("Software Engineer", job.Title);
            Assert.Equal(JobType.FullTime, job.JobType);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Job()
        {
            // Arrange
            var job = new Job
            {
                Title = "Data Scientist",
                Company = "AI Inc.",
                Description = "Analyze and interpret complex data sets, work with big data columes.",
                Location = "San Francisco",
                Salary = 220000,
                MinExperience = 8,
                MaxExperience = 20,
                WorkMode = WorkMode.OnSite,
                JobType = JobType.FullTime,
                Level = Level.Senior
            };

            context.Jobs.Add(job);
            await context.SaveChangesAsync();

            // Act
            var result = await jobService.GetByIdAsync(job.Id);

            // Assert
            Assert.Equal("Data Scientist", result.Title);
            Assert.Equal("AI Inc.", result.Company);
            Assert.Equal("Analyze and interpret complex data sets, work with big data columes.", result.Description);
            Assert.Equal("San Francisco", result.Location);
            Assert.Equal("OnSite", result.WorkMode);
            Assert.Equal("FullTime", result.JobType);
            Assert.Equal("Senior", result.Level);
            Assert.Equal(220000, result.Salary);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_If_Job_Not_Found()
        {
            // Arrange
            var job = new Job
            {
                Title = "Data Scientist",
                Company = "AI Inc.",
                Description = "Analyze and interpret complex data sets, work with big data columes.",
                Location = "San Francisco",
                Salary = 220000,
                MinExperience = 8,
                MaxExperience = 20,
                WorkMode = WorkMode.OnSite,
                JobType = JobType.FullTime,
                Level = Level.Senior
            };
            context.Jobs.Add(job);
            await context.SaveChangesAsync();

            // Act
            var result = await jobService.GetByIdAsync(2);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Job()
        {
            // Arrange
            var job = new Job
            {
                Title = "Data Scientist",
                Company = "AI Inc.",
                Description = "Analyze and interpret complex data sets, work with big data columes.",
                Location = "San Francisco",
                Salary = 220000,
                MinExperience = 8,
                MaxExperience = 20,
                WorkMode = WorkMode.OnSite,
                JobType = JobType.FullTime,
                Level = Level.Senior
            };
            context.Jobs.Add(job);
            await context.SaveChangesAsync();


            var updateModel = new JobFormModel
            {
                Id = 1,
                Title = "Data Analyst",
                Company = "AI Inc. 1",
                Description = "Just interpret complex data sets, work with big data volumes.",
                Location = "San Francisco, CA",
                Salary = 200000,
                MinExperience = 7,
                MaxExperience = 18,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.Contract,
                Level = (int)Level.Mid,
                EmployerId = 2
            };
            // Act
            await jobService.UpdateAsync(updateModel);
            var updatedJob = await context.Jobs.FirstOrDefaultAsync(j => j.Id == 1);

            // Assert
            Assert.NotNull(updatedJob);
            Assert.Equal("Data Analyst", updatedJob.Title);
            Assert.Equal("AI Inc. 1", updatedJob.Company);
            Assert.Equal("Just interpret complex data sets, work with big data volumes.", updatedJob.Description);
            Assert.Equal("San Francisco, CA", updatedJob.Location);
            Assert.Equal(200000, updatedJob.Salary);
            Assert.Equal(7, updatedJob.MinExperience);
            Assert.Equal(18, updatedJob.MaxExperience);
            Assert.Equal(WorkMode.Hybrid, updatedJob.WorkMode);
            Assert.Equal(JobType.Contract, updatedJob.JobType);
            Assert.Equal(Level.Mid, updatedJob.Level);
            Assert.Equal(2, updatedJob.EmployerId);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_If_Job_Not_Found()
        {
            // Arrange
            var updateModel = new JobFormModel
            {
                Id = 1,
                Title = "Data Analyst",
                Company = "AI Inc. 1",
                Description = "Just interpret complex data sets, work with big data volumes.",
                Location = "San Francisco, CA",
                Salary = 200000,
                MinExperience = 7,
                MaxExperience = 18,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.Contract,
                Level = (int)Level.Mid,
                EmployerId = 2
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => jobService.UpdateAsync(updateModel));
            Assert.Contains("Job not found", exception.Message);

        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Job()
        {
            // Arrange
            var job = new Job
            {
                Title = "Data Scientist",
                Company = "AI Inc.",
                Description = "Analyze and interpret complex data sets, work with big data volumes.",
                Location = "San Francisco",
                Salary = 220000,
                MinExperience = 8,
                MaxExperience = 20,
                WorkMode = WorkMode.OnSite,
                JobType = JobType.FullTime,
                Level = Level.Senior
            };
            context.Jobs.Add(job);
            await context.SaveChangesAsync();

            // Act
            await jobService.DeleteAsync(1);
            var deletedJob = await context.Jobs.FirstOrDefaultAsync(j => j.Id == 1);
            // Assert
            Assert.Null(deletedJob);
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_If_Job_Not_Found()
        {
            // Arrange
            var job = new Job
            {
                Title = "Data Scientist",
                Company = "AI Inc.",
                Description = "Analyze and interpret complex data sets, work with big data volumes.",
                Location = "San Francisco",
                Salary = 220000,
                MinExperience = 8,
                MaxExperience = 20,
                WorkMode = WorkMode.OnSite,
                JobType = JobType.FullTime,
                Level = Level.Senior
            };
            context.Jobs.Add(job);
            await context.SaveChangesAsync();
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => jobService.DeleteAsync(2));
            Assert.Contains("Job not found.", exception.Message);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
