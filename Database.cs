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
                    PRAGMA foreign_key = ON;

                CREATE TABLE IF NOT EXISTS WordleTable (
                        id INTEGER PRIMARY KEY,
                        solution TEXT NOT NULL,
                        print_date TEXT NOT NULL,
                        days_since_launch INTEGER NOT NULL,
                        editor TEXT NOT NULL
                        );

                CREATE TABLE IF NOT EXISTS ConnectionsTable (
                        id INTEGER PRIMARY KEY,
                        print_date TEXT NOT NULL,
                        editor TEXT NOT NULL,
                        status TEXT NOT NULL
                        );

                CREATE TABLE IF NOT EXISTS Categories (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        connection_id INTEGER NOT NULL,
                        title TEXT NOT NULL,
                        FOREIGN KEY (connection_id) REFERENCES ConnectionsTable(id) ON DELETE CASCADE
                        );


                CREATE TABLE IF NOT EXISTS Cards (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        category_id INTEGER NOT NULL,
                        content TEXT NOT NULL,
                        position INTEGER NOT NULL,
                        FOREIGN KEY (category_id) REFERENCES Categories(id) ON DELETE CASCADE
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
            }
        }

        public void insertIntoConnections(Connections connections) {
            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                using var transaction = connection.BeginTransaction();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR IGNORE INTO ConnectionsTable (id, print_date, editor, status) VALUES ($id, $print_date, $editor, $status);
                ";

                command.Parameters.AddWithValue("$id", connections.id);
                command.Parameters.AddWithValue("$print_date", connections.print_date);
                command.Parameters.AddWithValue("$editor", connections.editor);
                command.Parameters.AddWithValue("$status", connections.editor);

                command.ExecuteNonQuery();

                /* long connectionId = connection.LastInsertRowId; */
                var getConnId = connection.CreateCommand();
                getConnId.CommandText = "SELECT last_insert_rowid();";
                long connectionId = (long)getConnId.ExecuteScalar();


                foreach (var category in connections.categories) {
                    var catCommand = connection.CreateCommand();
                    catCommand.CommandText = @"
                        INSERT OR IGNORE INTO Categories (connection_id, title) VALUES ($connection_id, $title);
                    ";

                    catCommand.Parameters.AddWithValue("$connection_id", connectionId);
                    catCommand.Parameters.AddWithValue("$title", category.title);
                    catCommand.ExecuteNonQuery();

                    /* long categoryId = connection.LastInsertRowId; */
                    var getCatId = connection.CreateCommand();
                    getCatId.CommandText = "SELECT last_insert_rowid();";
                    long categoryId = (long)getCatId.ExecuteScalar();


                    foreach (var card in category.cards) {
                        var cardCommand = connection.CreateCommand();

                        cardCommand.CommandText = @"
                            INSERT OR IGNORE INTO Cards (category_id, content, position) VALUES ($category_id, $content, $position);
                        ";

                        cardCommand.Parameters.AddWithValue("$category_id", categoryId);
                        cardCommand.Parameters.AddWithValue("$content", card.content);
                        cardCommand.Parameters.AddWithValue("$position", card.position);

                        cardCommand.ExecuteNonQuery();

                    }
                }

                transaction.Commit();
            }

        }
    }
}
