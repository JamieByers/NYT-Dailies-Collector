using Dailies;
using Microsoft.Data.Sqlite;
using Dailies.models;

namespace Dailies {

    public class Database {
        private string connectionString = "Data source=dailies.db";

        public void createDBIfNotExists() {

            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                var createTablesCommand = connection.CreateCommand();
                createTablesCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS WordleTable (
                            id INTEGER PRIMARY KEY,
                            solution TEXT NOT NULL,
                            print_date TEXT NOT NULL,
                            days_since_launch INTEGER NOT NULL,
                            editor TEXT NOT NULL
                            );
                ";
                createTablesCommand.ExecuteNonQuery();
            }
        }

        public void insertIntoWordle(Wordle wordle) {
            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR IGNORE INTO WordleTable (id, solution, print_date, days_since_launch, editor)
                    VALUES ($id, $solution, $print_date, $days_since_launch, $editor);
                ";

                command.Parameters.AddWithValue("$id", wordle.id);
                command.Parameters.AddWithValue("$solution", wordle.solution);
                command.Parameters.AddWithValue("$print_date", wordle.print_date);
                command.Parameters.AddWithValue("$days_since_launch", wordle.days_since_launch);
                command.Parameters.AddWithValue("$editor", wordle.editor);

                command.ExecuteNonQuery();
            }}

    }
}
