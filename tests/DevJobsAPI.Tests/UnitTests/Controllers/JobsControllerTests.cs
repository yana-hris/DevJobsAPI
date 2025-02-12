namespace DevJobsAPI.Tests.UnitTests.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    using DevJobsAPI.Controllers;
    using DevJobsAPI.Services.Interfaces;
    using DevJobsAPI.ViewModels;

    public class JobsControllerTests
    {

        private readonly JobsController controller;
        private readonly Mock<IJobService> jobServiceMock = new();

        public JobsControllerTests()
        {
            controller = new JobsController(jobServiceMock.Object);
        }

        [Fact]
        public async Task GetJobs_Returns_Ok_With_JobsList()
        {
            // Arrange
            var jobs = new List<JobViewModel>
            {
                new JobViewModel
                {
                    Title = "Software Developer",
                    Description = "We are looking for a software developer to join our team.",
                    Company = "Company A",
                    Location = "Sofia",
                    MinExperience = 2,
                    MaxExperience = 5,
                    WorkMode = "Remote",
                    JobType = "FullTime",
                    Level = "Senior",
                    Salary = 5000
                },
                new JobViewModel
                {
                    Title = "QA Engineer",
                    Description = "We are looking for a QA engineer to join our team.",
                    Company = "Company B",
                    Location = "Plovdiv",
                    MinExperience = 1,
                    MaxExperience = 3,
                    WorkMode = "OnSite",
                    JobType = "PartTime",
                    Level = "Junior",
                    Salary = 3000
                }
            };

            jobServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(jobs);

            // Act
            var result = await controller.GetJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<JobViewModel>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        // GetJob() => returns NotFound if id is not found

        // GetJob() => returns Ok with job if id is found

        // CreateJob() => returns BadRequest if ModelState is invalid

        // CreateJob() => returns CreatedAtAction with job if job is created successfully

        // CreateJob() => returns BadRequest if an exception is thrown

        // UpdateJob() => returns BadRequest if id is different from job.Id

        // UpdateJob() => returns BadRequest if ModelState is invalid

        // UpdateJob() => returns Ok if job is updated successfully

        // UpdateJob() => returns NotFound if job is not found

        // DeleteJob() => returns NotFound if job is not found

        // DeleteJob() => returns BadRequest if an exception is thrown

        // DeleteJob() => returns Ok if job is deleted successfully

    }
}
