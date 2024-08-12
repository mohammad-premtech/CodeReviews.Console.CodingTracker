using CodingTracker.Models;
using CodingTracker.Services;
using CodingTracker.Utilities;
using System.Data.SQLite;
using System.Diagnostics;
using Dapper;

namespace CodingTracker.Controllers
{
    public class CodingController
    {
        private readonly DatabaseService _databaseService;

        public CodingController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddSession()
        {
            DateTime startTime = UserInput.GetDateTime("Enter start time (yyyy-MM-dd HH:mm): ");
            DateTime endTime;
            do
            {
                endTime = UserInput.GetDateTime("Enter end time (yyyy-MM-dd HH:mm): ");
                if (endTime <= startTime)
                {
                    Console.WriteLine("End time cannot be before start time. Please re-enter the end time.");
                }
            } while (endTime <= startTime);

            var session = new CodingSession(startTime, endTime);
            _databaseService.AddCodingSession(session);
            Console.WriteLine("Coding session added successfully.");
        }

        public void DisplaySessions()
        {
            var sessions = _databaseService.GetCodingSessions();
            TableVisualisationEngine.RenderSessions(sessions);
        }

        public void StartSessionWithStopWatch()
        {
            Console.WriteLine("Press Enter to start the stop watch..");
            Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Stopwatch started. Press Enter to stop..");
            Console.ReadLine();

            stopwatch.Stop();

            DateTime startTime = DateTime.Now - stopwatch.Elapsed;
            DateTime endTime = DateTime.Now;

            var session = new CodingSession(startTime, endTime);
            _databaseService.AddCodingSession(session); 

            Console.WriteLine($"Coding session tracked successfully. Duration: {stopwatch.Elapsed}");

        }

        public void FilterAndOrderSessions()
        {
            Console.WriteLine("Choose filter period:");
            Console.WriteLine("1. By Day");
            Console.WriteLine("2. By Week");
            Console.WriteLine("3. By Year");
            string periodChoice = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Order by:");
            Console.WriteLine("1. Ascending");
            Console.WriteLine("2. Descending");
            string orderChoice = Console.ReadLine() ?? string.Empty;

            List<CodingSession> sessions = _databaseService.GetCodingSessions();

            IEnumerable<CodingSession> filteredSessions = sessions;

            switch (periodChoice)
            {
                case "1": // By Day
                    filteredSessions = orderChoice == "1"
                        ? filteredSessions.OrderBy(session => DateTime.Parse(session.StartTime).Date)
                                          .ThenBy(session => DateTime.Parse(session.StartTime))
                        : filteredSessions.OrderByDescending(session => DateTime.Parse(session.StartTime).Date)
                                          .ThenByDescending(session => DateTime.Parse(session.StartTime));
                    break;

                case "2": // By Week
                    filteredSessions = orderChoice == "1"
                        ? filteredSessions.OrderBy(session => GetWeekOfYear(DateTime.Parse(session.StartTime)))
                                          .ThenBy(session => DateTime.Parse(session.StartTime))
                        : filteredSessions.OrderByDescending(session => GetWeekOfYear(DateTime.Parse(session.StartTime)))
                                          .ThenByDescending(session => DateTime.Parse(session.StartTime));
                    break;

                case "3": // By Year
                    filteredSessions = orderChoice == "1"
                        ? filteredSessions.OrderBy(session => DateTime.Parse(session.StartTime).Year)
                                          .ThenBy(session => DateTime.Parse(session.StartTime))
                        : filteredSessions.OrderByDescending(session => DateTime.Parse(session.StartTime).Year)
                                          .ThenByDescending(session => DateTime.Parse(session.StartTime));
                    break;

                default:
                    Console.WriteLine("Invalid period choice.");
                    return;
            }

            TableVisualisationEngine.RenderSessions(filteredSessions.ToList());
        }

        private int GetWeekOfYear(DateTime date)
        {
            var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            return cultureInfo.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public void GenerateReports()
        {
            Console.WriteLine("Choose report period:");
            Console.WriteLine("1. By Day");
            Console.WriteLine("2. By Week");
            Console.WriteLine("3. By Year");
            string periodChoice = Console.ReadLine() ?? string.Empty;

            List<CodingSession> sessions = _databaseService.GetCodingSessions();

            switch (periodChoice)
            {
                case "1": // By Day
                    var groupedByDay = sessions.GroupBy(session => DateTime.Parse(session.StartTime).Date);
                    DisplayReport(groupedByDay);
                    break;
                case "2": // By Week
                    var groupedByWeek = sessions.GroupBy(session =>
                        DateTime.Parse(session.StartTime).Date.AddDays(-(int)DateTime.Parse(session.StartTime).DayOfWeek));
                    DisplayReport(groupedByWeek);
                    break;
                case "3": // By Year
                    var groupedByYear = sessions.GroupBy(session => DateTime.Parse(session.StartTime).Year);
                    DisplayReport(groupedByYear);
                    break;
                default:
                    Console.WriteLine("Invalid period choice.");
                    return;
            }
        }

        private void DisplayReport<TKey>(IEnumerable<IGrouping<TKey, CodingSession>> groupedSessions)
        {
            Console.WriteLine("Report:");
            foreach (var group in groupedSessions)
            {
                var totalDuration = group.Sum(session => session.Duration.TotalHours);
                var averageDuration = group.Average(session => session.Duration.TotalHours);
                Console.WriteLine($"Period: {group.Key}");
                Console.WriteLine($"Total Coding Time: {totalDuration:F2} hours");
                Console.WriteLine($"Average Coding Time: {averageDuration:F2} hours");
                Console.WriteLine(new string('-', 40));
            }
        }

        public void ManageCodingGoals()
        {
            Console.WriteLine("1. Set a New Goal");
            Console.WriteLine("2. View Progress Towards Goals");
            string goalChoice = Console.ReadLine() ?? string.Empty;

            switch (goalChoice)
            {
                case "1":
                    SetCodingGoal();
                    break;
                case "2":
                    ViewCodingGoalProgress();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void SetCodingGoal()
        {
            Console.WriteLine("Enter goal name:");
            string goalName = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter target hours:");
            double targetHours = double.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter start date (yyyy-MM-dd):");
            string startDate = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter end date (yyyy-MM-dd):");
            string endDate = Console.ReadLine() ?? string.Empty;

            using (var connection = new SQLiteConnection(_databaseService.ConnectionString))
            {
                connection.Execute("INSERT INTO CodingGoals (GoalName, TargetHours, StartDate, EndDate) VALUES (@GoalName, @TargetHours, @StartDate, @EndDate)",
                    new { GoalName = goalName, TargetHours = targetHours, StartDate = startDate, EndDate = endDate });
            }

            Console.WriteLine("Goal set successfully.");
        }

        private void ViewCodingGoalProgress()
        {
            List<CodingGoal> goals;
            using (var connection = new SQLiteConnection(_databaseService.ConnectionString))
            {
                goals = connection.Query<CodingGoal>("SELECT * FROM CodingGoals").ToList();
            }

            foreach (var goal in goals)
            {
                double totalCodingHours = GetTotalCodingHours(goal.StartDate, goal.EndDate);
                double hoursLeft = goal.TargetHours - totalCodingHours;
                int daysLeft = (DateTime.Parse(goal.EndDate) - DateTime.Today).Days;
                double hoursPerDay = hoursLeft > 0 ? hoursLeft / daysLeft : 0;

                
                goal.ExtraHours = totalCodingHours > goal.TargetHours ? totalCodingHours - goal.TargetHours : 0;

                Console.WriteLine($"Goal: {goal.GoalName}");
                Console.WriteLine($"Target Hours: {goal.TargetHours:F2}");
                Console.WriteLine($"Target Dates: {goal.StartDate} to {goal.EndDate}");
                Console.WriteLine($"Total Hours Coded: {totalCodingHours:F2}");
                Console.WriteLine($"Hours Left: {(hoursLeft > 0 ? hoursLeft : 0):F2}");
                Console.WriteLine($"Days Left: {(daysLeft > 0 ? daysLeft : 0)}");
                Console.WriteLine($"Hours Per Day Needed to Meet Goal: {hoursPerDay:F2}");

                if (goal.ExtraHours > 0)
                {
                    Console.WriteLine($"Extra Hours Done: {goal.ExtraHours:F2}");
                }

               
                using (var connection = new SQLiteConnection(_databaseService.ConnectionString))
                {
                    connection.Execute("UPDATE CodingGoals SET ExtraHours = @ExtraHours WHERE Id = @Id", new { ExtraHours = goal.ExtraHours, Id = goal.Id });
                }

                Console.WriteLine(new string('-', 40));
            }
        }



        private double GetTotalCodingHours(string startDate, string endDate)
        {
            List<CodingSession> sessions = _databaseService.GetCodingSessions();
            var filteredSessions = sessions.Where(session =>
                DateTime.Parse(session.StartTime) >= DateTime.Parse(startDate) &&
                DateTime.Parse(session.EndTime) <= DateTime.Parse(endDate));

            return filteredSessions.Sum(session => session.Duration.TotalHours);
        }




    }
}
