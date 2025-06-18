using System;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Dailies.models;
// https://www.nytimes.com/svc/connections/v2/%s.json
// https://www.nytimes.com/svc/wordle/v2/%s.json
// https://www.nytimes.com/svc/crosswords/v6/editorial-content/puzzle/{date}.json

class Fetcher {
    private static HttpClient httpClient = new();

    static async Task Main(string[] args) {
        Database db = new Database();
        db.createDBIfNotExists();

        string date = Fetcher.Date();

        string wordleUrl = $"https://www.nytimes.com/svc/wordle/v2/{date}.json";
        /* string miniUrl = $"https://www.nytimes.com/svc/crosswords/v6/editorial-content/puzzle/{date}.json"; */
        /* string connectionsUrl = $"https://www.nytimes.com/svc/connections/v2/{date}.json"; */

        string? wordleResponse = await Fetch(wordleUrl);
        if (wordleResponse is not null) {
            Wordle? wordle = getWordle(wordleResponse);
            if (wordle is not null) {
                wordle.Display();
                db.insertIntoWordle(wordle);
            }
        }

    }

    static string Date() {
        DateTime now = DateTime.Now;
        string formatted_date = now.ToString("yyyy/MM/dd");
        formatted_date = formatted_date.Replace("/", "-");
        return formatted_date;
    }

    static async Task<string?> Fetch(string url) {
        try {
            string response = await httpClient.GetStringAsync(url);
            Console.WriteLine(response);
            return response;
        } catch (HttpRequestException e) {
            Console.WriteLine($"Could not fetch daily: {url} - {e}");
            return null;
        }
    }

    static Wordle? getWordle(string response) {
            if (response != null) {
                Wordle? wordle = JsonSerializer.Deserialize<Wordle>(response);
                if (wordle != null) {
                    wordle.date = DateTime.Now;
                    return wordle;
                }
            }


            return null;
    }
}

public class Daily {
    public int id { get; set; }
    public DateTime date { get; set; }
    public string editor { get; set; } = "";

}

public class Wordle : Daily {
    public string solution { get; set; } = "";
    public string print_date { get; set; } = "";
    public int days_since_launch { get; set; }

    public void Display() {
        Console.WriteLine($"id: {this.id}");
        Console.WriteLine($"solution: {this.solution}");
        Console.WriteLine($"print_date: {this.print_date}");
        Console.WriteLine($"date: {this.date}");
        Console.WriteLine($"editor: {this.editor}");
    }
}

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
