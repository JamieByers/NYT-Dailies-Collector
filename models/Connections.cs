using Dailies.models;

namespace Dailies
{
    public class Connections : Daily {
        public string status { get; set; } = "";
        public List<Category> categories { get; set; } = new List<Category>();

        public void Display()
        {
            Console.WriteLine($"Status: {status}");
            Console.WriteLine("Categories:");
            foreach (var category in categories)
            {
                Console.WriteLine($"  - {category.title}");
                foreach (var card in category.cards)
                {
                    Console.WriteLine($"      • {card.content} (Position: {card.position})");
                }
            }
        }
   }

   public class Category {
        public List<Card> cards {get; set;} = new List<Card>();
        public string title {get; set;} = "";
   }

   public class Card {
        public string content { get; set; } = "";
        public int position { get; set; }
   }
}
