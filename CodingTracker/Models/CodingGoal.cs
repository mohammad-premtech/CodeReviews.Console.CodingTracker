namespace CodingTracker.Models
{
    public class CodingGoal
    {
        public int Id { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public double TargetHours { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public double ExtraHours { get; set; }
    }
}
