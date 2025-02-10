namespace DevJobsAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using DevJobsAPI.Data;
    using DevJobsAPI.Models;
    using Microsoft.AspNetCore.Authorization;
    using DevJobsAPI.ViewModels;
    using DevJobsAPI.Models.Enums;

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public JobsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await context.Jobs.ToListAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(long id)
        {
            var job = await context.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound(new { message = $"Job with id {id} not found" });
            }

            return Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobFormModel job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return CreatedAtAction(nameof(GetJob), new {id = newJob.Id }, newJob);
        }

        [HttpPut("{id}")]   
        public async Task<IActionResult> UpdateJob(long id, [FromBody] Job job)
        {
            if(id != job.Id)
            {
                return BadRequest(new { message = "Job ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            context.Entry(job).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Jobs.Any(e => e.Id == id))
                    return NotFound(new { message = $"Job with id {id} not found" });

                throw;
            }

            return Ok(new { message = "Job updated successfully" }); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(long id)
        {
            var job = await context.Jobs.FindAsync(id);

            if(job == null)
            {
                return NotFound(new { message = $"Job with id {id} not found" });
            }

            context.Jobs.Remove(job);
            await context.SaveChangesAsync();

            return Ok(new { message = "Job deleted successfully" });    
        }
    }
}
