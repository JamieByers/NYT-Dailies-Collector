using System;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Dailies.models;
using Dailies;
// https://www.nytimes.com/svc/connections/v2/%s.json
// https://www.nytimes.com/svc/wordle/v2/%s.json
// https://www.nytimes.com/svc/crosswords/v6/editorial-content/puzzle/{date}.json


class DailiesCollector {
    static async Task Main(string[] args) {
        Database db = new Database();
        db.createDBIfNotExists();

        Fetcher fetcher = new Fetcher();

        Wordle? wordle = await Fetcher.handleWordle();
        if (wordle is not null) {
            /* wordle.Display(); */
            db.insertIntoWordle(wordle);
        }

        Connections? connections = await Fetcher.handleConnections();
        if (connections is not null) {
            /* connections.Display(); */
            db.insertIntoConnections(connections);
        }

    }
}

