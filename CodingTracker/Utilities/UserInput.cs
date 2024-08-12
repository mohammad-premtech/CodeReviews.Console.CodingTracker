namespace CodingTracker.Utilities
{

public static class UserInput
    {
        public static DateTime GetDateTime(string prompt)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine() ?? string.Empty;
            DateTime result;
            while(!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out result))
            {
                Console.WriteLine("Invalid format. Enter in 'yyyy-MM-dd HH:mm' format");
                input = Console.ReadLine() ?? string.Empty;
            }
            return result;
        }
    }


}