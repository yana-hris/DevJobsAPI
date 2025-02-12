namespace DevJobsAPI.ViewModels
{    
    public class JobViewModel
    {        
        public string Title { get; set; } = null!;
        
        public string Description { get; set; } = null!;
        
        public string Company { get; set; } = null!;
        
        public string Location { get; set; } = null!;
        
        public int MinExperience { get; set; }
       
        public int MaxExperience { get; set; }
        
        public string? WorkMode { get; set; }

        public string JobType { get; set; } = null!;
        
        public string? Level { get; set; }  
        
        public decimal Salary { get; set; }
        
    }
}
