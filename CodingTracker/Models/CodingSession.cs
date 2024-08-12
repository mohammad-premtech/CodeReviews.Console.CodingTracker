namespace CodingTracker.Models
{
    public class CodingSession
    {
        public int Id { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public TimeSpan Duration
        {
            get
            {
                DateTime start = DateTime.Parse(StartTime);
                DateTime end = DateTime.Parse(EndTime);
                return end - start;
            }
        }

     
        public CodingSession()
        {
            StartTime = string.Empty;
            EndTime = string.Empty;
        }

        public CodingSession(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime.ToString("yyyy-MM-dd HH:mm");
            EndTime = endTime.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
