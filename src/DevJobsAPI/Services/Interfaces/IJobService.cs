namespace DevJobsAPI.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DevJobsAPI.ViewModels;

    public interface IJobService
    {
        Task<long> AddAsync(JobFormModel job);
        Task DeleteAsync(long id);
        Task<ICollection<JobViewModel>> GetAllAsync();
        Task<JobViewModel?> GetByIdAsync(long id);
        Task UpdateAsync(JobFormModel job);
    }
}
