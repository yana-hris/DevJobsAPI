namespace DevJobsAPI.Tests.UnitTests.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    using DevJobsAPI.Controllers;
    using DevJobsAPI.Services.Interfaces;
    using DevJobsAPI.ViewModels;
    using DevJobsAPI.Models.Enums;
    using Microsoft.EntityFrameworkCore;

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

        [Fact]
        public async Task GetJob_Should_Return_NotFound_If_Job_NotFound()
        {
            // Arrange
            long jobId = 1;
            jobServiceMock
                .Setup(s => s.GetByIdAsync(jobId))
                .ReturnsAsync((JobViewModel?)null);

            // Act
            var result = await controller.GetJob(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseObject = notFoundResult.Value;

            Assert.NotNull(responseObject);
            
            var messageProperty = responseObject.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);

            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal($"Job with id {jobId} not found", actualMessage);

        }

        [Fact]
        public async Task GetJob_Should_Return_Ok_With_Job_If_Found()
        {
            // Arrange
            long jobId = 1;
            var job = new JobViewModel
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
            };
            jobServiceMock
                .Setup(s => s.GetByIdAsync(jobId))
                .ReturnsAsync(job);

            // Act
            var result = await controller.GetJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var model = Assert.IsType<JobViewModel>(okResult.Value);

            Assert.Equal(job.Title, model.Title);
            Assert.Equal(job.Description, model.Description);
            Assert.Equal(job.Company, model.Company);
            Assert.Equal(job.Location, model.Location);
            Assert.Equal(job.MinExperience, model.MinExperience);
            Assert.Equal(job.MaxExperience, model.MaxExperience);
            Assert.Equal(job.WorkMode, model.WorkMode);
            Assert.Equal(job.JobType, model.JobType);
            Assert.Equal(job.Level, model.Level);
            Assert.Equal(job.Salary, model.Salary);
        }
        

        [Fact]
        public async Task CreateJob_Should_Return_BadRequest_If_ModelState_Is_Invalid()
        {
            // Arrange
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.CreateJob(new JobFormModel());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("Title"));
        }

        
        [Fact]
        public async Task CreateJob_Should_Return_CreateAtAction_With_Job_If_Successful()
        {
            // Assert
            var jobFormModel = new JobFormModel
            {
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            long newJobId = 1;

            var createdJobModel = new JobViewModel
            {
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = WorkMode.Hybrid.ToString(),
                JobType = JobType.FullTime.ToString(),
                Level = Level.Mid.ToString(),
                Salary = 5000,
            };

            jobServiceMock
                .Setup(s => s.AddAsync(jobFormModel))
                .ReturnsAsync(newJobId);

            jobServiceMock
                .Setup(s => s.GetByIdAsync(newJobId))
                .ReturnsAsync(createdJobModel);

            // Act
            var result = await controller.CreateJob(jobFormModel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetJob), createdAtActionResult.ActionName);

            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.True(createdAtActionResult.RouteValues.ContainsKey("id"));
            Assert.Equal(newJobId, createdAtActionResult.RouteValues["id"]);

            var returnedJob = Assert.IsType<JobViewModel>(createdAtActionResult.Value);
            Assert.Equal(returnedJob.Title, jobFormModel.Title);

        }
        [Fact]
        public async Task CreateJob_Should_Return_BadRequest_If_Exception_Is_Thrown()
        {
            // Arrange
            var jobFormModel = new JobFormModel
            {
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            jobServiceMock
                .Setup(s => s.AddAsync(jobFormModel))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await controller.CreateJob(jobFormModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseObject = badRequestResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);
            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal("Error creating job", actualMessage);

        }

        
        [Fact]
        public async Task UpdateJob_Should_Return_BadRequest_If_Id_Differs_From_JobId()
        {
            // Arrange
            long jobId = 1;

            var jobFormModel = new JobFormModel
            {
                Id = 2,
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            // Act
            var result = await controller.UpdateJob(jobId, jobFormModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);   
            var responseObject = badRequestResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);
            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal("Job ID mismatch", actualMessage);
        }

        
        [Fact]
        public async Task UpdateJob_Should_Return_BadRequest_If_ModelState_Is_Invalid()
        {
            // Arrange
            long jobId = 1;
            var jobFormModel = new JobFormModel
            {
                Id = jobId,
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.UpdateJob(jobId, jobFormModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("Title"));
        }

        
        [Fact]
        public async Task UpdateJob_Should_Return_Ok_If_Job_Updated_Successfully()
        {
            // Arrange
            long jobId = 1;

            var jobFormModel = new JobFormModel
            {
                Id = jobId,
                Title = "Software Developer",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            // Simulate successful update
            jobServiceMock
                .Setup(s => s.UpdateAsync(jobFormModel))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.UpdateJob(jobId, jobFormModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal("Job updated successfully", actualMessage);
        }

        
        [Fact]
        public async Task UpdateJob_Should_Return_NotFound_If_Not_Found()
        {
            // Arrange
            long jobId = 1;
            var jobFormModel = new JobFormModel
            {
                Id = jobId,
                Title = "Non-existing Job",
                Description = "We are looking for a software developer to join our team of professionals.",
                Company = "Company A",
                Location = "Sofia",
                MinExperience = 2,
                MaxExperience = 5,
                WorkMode = (int)WorkMode.Hybrid,
                JobType = (int)JobType.FullTime,
                Level = (int)Level.Mid,
                Salary = 5000,
                EmployerId = 1
            };

            jobServiceMock
                .Setup(s => s.UpdateAsync(jobFormModel))
                .ThrowsAsync(new ArgumentNullException());

            // Act
            var result = await controller.UpdateJob(jobId, jobFormModel);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var resonseObject = notFoundResult.Value;
            var messageProperty = resonseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);
            string actualMessage = messageProperty.GetValue(resonseObject)?.ToString();
            Assert.Equal("Job not found", actualMessage);

        }

        // DeleteJob() => returns NotFound if job is not found

        [Fact]
        public async Task DeleteJob_Should_Return_NotFound_If_Job_NotFound()
        {
            // Arrange
            long jobId = 1;
            jobServiceMock
                .Setup(s => s.DeleteAsync(jobId))
                .ThrowsAsync(new ArgumentNullException());

            // Act
            var result = await controller.DeleteJob(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var responseObject = notFoundResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);

            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal($"Job with id {jobId} not found", actualMessage);
        }

        
        [Fact]
        public async Task DeleteJob_Should_Return_Ok_If_Job_Deleted_Successfully()
        {
            // Arrange
            long jobId = 1;

            // Simulate successful deletion
            jobServiceMock
                .Setup(s => s.DeleteAsync(jobId))
                .Returns(Task.CompletedTask);


            // Act
            var result = await controller.DeleteJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var responseObject = okResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);
            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal("Job deleted successfully", actualMessage);
        }

        
        [Fact]
        public async Task DeleteJob_Should_Return_BadRequest_If_Exception_Is_Thrown()
        {
            // Arrange
            long jobId = 1;
            jobServiceMock
                .Setup(s => s.DeleteAsync(jobId))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await controller.DeleteJob(jobId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var responseObject = badRequestResult.Value;
            var messageProperty = responseObject.GetType().GetProperty("message");

            Assert.NotNull(messageProperty);

            string actualMessage = messageProperty.GetValue(responseObject)?.ToString();
            Assert.Equal("Error deleting job", actualMessage);
        }

    }
}
