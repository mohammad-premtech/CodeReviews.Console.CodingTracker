namespace CodingTracker.Utilities
{
    public static class Validation
    {
        public static bool ValidateMenuChoice(string choice, int maxOption)
        {
            int option;
            if (int.TryParse(choice, out option) && option >= 1 && option <= maxOption)
            {
                return true;
            }
            Console.WriteLine("Invalid option. Please enter a number between 1 and " + maxOption);
            return false;
        }

        public static bool ValidateDates(DateTime start, DateTime end)
        {
            if (end > start) return true;
            Console.WriteLine("End date cannot be before the start date.");
            return false;
        }
    }
}
