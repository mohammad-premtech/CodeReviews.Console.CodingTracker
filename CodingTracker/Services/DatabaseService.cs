using System.Data.SQLite;
using Dapper;
using CodingTracker.Models;

namespace CodingTracker.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        public string ConnectionString => _connectionString;

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                
                string createSessionsTable = @"CREATE TABLE IF NOT EXISTS CodingSessions (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        StartTime TEXT NOT NULL,
                                        EndTime TEXT NOT NULL,
                                        Duration TEXT NOT NULL)";
                using (var cmd = new SQLiteCommand(createSessionsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

               
                string createGoalsTable = @"CREATE TABLE IF NOT EXISTS CodingGoals (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    GoalName TEXT NOT NULL,
                                    TargetHours REAL NOT NULL,
                                    StartDate TEXT NOT NULL,
                                    EndDate TEXT NOT NULL,
                                    ExtraHours REAL DEFAULT 0)";
                using (var cmd = new SQLiteCommand(createGoalsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                
                string addExtraHoursColumn = @"ALTER TABLE CodingGoals ADD COLUMN ExtraHours REAL DEFAULT 0";
                try
                {
                    using (var cmd = new SQLiteCommand(addExtraHoursColumn, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    
                    if (!ex.Message.Contains("duplicate column name"))
                    {
                        throw;
                    }
                }
            }
        }


        public void AddCodingSession(CodingSession session)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Execute("INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES (@StartTime, @EndTime, @Duration)", session);
            }
        }

        public List<CodingSession> GetCodingSessions()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<CodingSession>("SELECT * FROM CodingSessions").ToList();
            }
        }

   
        public void AddCodingGoal(CodingGoal goal)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Execute("INSERT INTO CodingGoals (GoalName, TargetHours, StartDate, EndDate) VALUES (@GoalName, @TargetHours, @StartDate, @EndDate)", goal);
            }
        }

    
        public List<CodingGoal> GetCodingGoals()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<CodingGoal>("SELECT * FROM CodingGoals").ToList();
            }
        }
    }
}
