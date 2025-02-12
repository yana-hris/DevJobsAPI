namespace DevJobsAPI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    using DevJobsAPI.ViewModels;
    using DevJobsAPI.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class JobsController : ControllerBase
    {
        private readonly IJobService jobService;

        public JobsController(IJobService jobService)
        {
            this.jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await jobService.GetAllAsync();  
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(long id)
        {
            var job = await jobService.GetByIdAsync(id);

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

            try
            {
                long id = await jobService.AddAsync(job);
                var createdJob = await jobService.GetByIdAsync(id);
                return CreatedAtAction(nameof(GetJob), new { id }, createdJob);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error creating job" });
            }
        }

        [HttpPut("{id}")]   
        public async Task<IActionResult> UpdateJob(long id, [FromBody] JobFormModel job)
        {
            if(id != job.Id)
            {
                return BadRequest(new { message = "Job ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                await jobService.UpdateAsync(job);
            }
            catch (ArgumentNullException)
            {
                return NotFound(new { message = "Job not found" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error updating job" });
            }

            return Ok(new { message = "Job updated successfully" }); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(long id)
        {
            try
            {
                await jobService.DeleteAsync(id);
            }
            catch (ArgumentNullException)
            {
                return NotFound(new { message = $"Job with id {id} not found" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error deleting job" });
            }            

            return Ok(new { message = "Job deleted successfully" });    
        }
    }
}
