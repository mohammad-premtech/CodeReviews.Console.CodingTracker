using CodingTracker.Controllers;
using CodingTracker.Services;
using CodingTracker.Utilities;

namespace CodingTracker
{
    class Program
    {
        static void Main(string[] args) 
        {
            var connectionString = Configuration.GetConnectionString();
            var databaseService = new DatabaseService(connectionString);
            var codingController = new CodingController(databaseService);

            while (true)
            {
                Console.WriteLine("1. Add Coding Session Manually");
                Console.WriteLine("2. View Coding Sessions");
                Console.WriteLine("3. Start a Coding Session with Stopwatch");
                Console.WriteLine("4. Filter and Order Coding Sessions");
                Console.WriteLine("5. Generate Reports");
                Console.WriteLine("6. Set and View Coding Goals");
                Console.WriteLine("7. Exit");

               
                string choice = Console.ReadLine() ?? string.Empty;

                if (Validation.ValidateMenuChoice(choice, 7))
                {
                    switch (choice)
                    {
                        case "1":
                            codingController.AddSession();
                            break;
                        case "2":
                            codingController.DisplaySessions();
                            break;
                        case "3":
                            codingController.StartSessionWithStopWatch();
                            break;
                        case "4":
                            codingController.FilterAndOrderSessions();
                            break;
                        case "5":
                            codingController.GenerateReports();
                            break;
                        case "6":
                            codingController.ManageCodingGoals();
                            break;
                        case "7":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please select again.");
                            break;
                    }
                }
            }
        }
    }
}
