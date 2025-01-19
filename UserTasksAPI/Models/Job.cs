namespace UserTasksAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Assignee { get; set; }
        public DateTime DueDate { get; set; }
    }
}