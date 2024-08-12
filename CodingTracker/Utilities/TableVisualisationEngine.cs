using Spectre.Console;
using CodingTracker.Models;

namespace CodingTracker.Utilities
{
    public static class TableVisualisationEngine
    {
        public static void RenderSessions(List<CodingSession> sessions)
        {
            var table = new Table().AddColumn("ID")
                                   .AddColumn("Start Time")
                                   .AddColumn("End Time")
                                   .AddColumn("Duration");

            foreach (var session in sessions)
            {
                table.AddRow(session.Id.ToString(),
                             session.StartTime,
                             session.EndTime,
                             session.Duration.ToString());
            }

            AnsiConsole.Write(table);
        }
    }
}
