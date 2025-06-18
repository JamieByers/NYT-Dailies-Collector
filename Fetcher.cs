using System;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Dailies.models;
using Dailies;
// https://www.nytimes.com/svc/connections/v2/%s.json
// https://www.nytimes.com/svc/wordle/v2/%s.json
// https://www.nytimes.com/svc/crosswords/v6/editorial-content/puzzle/{date}.json

namespace Dailies {
    class Fetcher {
        private static HttpClient httpClient = new();
        public static string date = Fetcher.Date();

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

        static string Date() {
            DateTime now = DateTime.Now;
            string formatted_date = now.ToString("yyyy/MM/dd");
            formatted_date = formatted_date.Replace("/", "-");
            return formatted_date;
        }

        static Wordle? getWordle(string response) {
            if (response is not null) {
                Wordle? wordle = JsonSerializer.Deserialize<Wordle>(response);
                if (wordle != null) {
                    return wordle;
                }
            }

            return null;
        }

        static public async Task<Wordle?> handleWordle() {
            string wordleUrl = $"https://www.nytimes.com/svc/wordle/v2/{Fetcher.date}.json";

            string? wordleResponse = await Fetch(wordleUrl);
            if (wordleResponse is not null) {
                Wordle? wordle = getWordle(wordleResponse);
                if (wordle is not null) {
                    wordle.Display();
                    return wordle;
                }
            }
            return null;
        }

        static public Connections? getConnections(string response) {
            if (response is not null) {
                Connections? connections = JsonSerializer.Deserialize<Connections>(response);
                if (connections is not null) {
                    return connections;
                }
            }

            return null;
        }

        static public async Task<Connections?> handleConnections() {
            string connectionsUrl = $"https://www.nytimes.com/svc/connections/v2/{Fetcher.date}.json";

            string? connectionsResponse = await Fetch(connectionsUrl);
            if (connectionsResponse is not null) {
                Connections? connections = getConnections(connectionsResponse);
                if (connections is not null) {
                    return connections;
                }
            }

            return null;
        }
    }
}
