using Dailies.models;

namespace Dailies.models {
    public class Wordle : Daily {
        public string solution { get; set; } = "";
        public int days_since_launch { get; set; }

        public void Display() {
            Console.WriteLine($"id: {this.id}");
            /* Console.WriteLine($"solution: {this.solution}"); */
            Console.WriteLine($"solution: *****");
            Console.WriteLine($"print_date: {this.print_date}");
            Console.WriteLine($"date: {this.date}");
            Console.WriteLine($"editor: {this.editor}");
        }
    }
}
