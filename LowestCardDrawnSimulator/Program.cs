using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public enum Suit
{
    Spades = 1,
    Clubs,
    Diamonds,
    Hearts
}

public enum Value
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public class Card
{
    private string name;
    public Suit Suit { get; set; }
    public Value Value { get; set; }

    public Card(Suit suit, Value value)
    {
        Suit = suit;
        Value = value;
    }

    public string Name
    {
        get
        {
            if (name == null)
            {
                name = $"{Value.ToString()} of {Suit.ToString()}";
            }

            return name;
        }
    }
}

//Shuffle method borrowed from https://stackoverflow.com/questions/273313/randomize-a-listt

public static class ThreadSafeRandom
{
    [ThreadStatic] private static Random Local;

    public static Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }
}

static class ListExtension
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

//End shuffle method

public class Program
{
    List<Card> deck;
    int[] dealtLowestCard;

    static void Main(string[] args)
    {
        Program program = new Program();
        program.Run();
    }

    static string NumberSuffix(int n)
    {
        int lastDigit = n % 10;

        switch (lastDigit)
        {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }

    private void Run()
    {
        string input = "";
        int numCardsDealt = 0;
        int iterations = 0;
        float progress = 0.0f;

        Console.WriteLine($"How many cards do you want to deal? Please choose a number between 1 and 52.");

        do
        {
            input = Console.ReadLine();

            if (!Int32.TryParse(input, out numCardsDealt))
            {
                Console.WriteLine("That is not a number. Please choose a number of cards to deal between 1 and 52.");
            }
            else if (numCardsDealt < 0 || numCardsDealt > 52)
            {
                Console.WriteLine($"Cannot deal {numCardsDealt} cards from a 52-card deck. Please choose a number of cards to deal between 1 and 52.");
            }
            else if (numCardsDealt == 0)
            {
                Console.WriteLine("Please choose a number of cards to deal between 1 and 52.");
            }
        }
        while (numCardsDealt <= 0 || numCardsDealt > 52);

        Console.WriteLine($"How many times would you like to simulate dealing {numCardsDealt} cards? Please enter a number greater than 0.");

        do
        {
            input = Console.ReadLine();

            if (!Int32.TryParse(input, out iterations))
            {
                Console.WriteLine("That is not a number. Please enter a number greater than 0.");
            }
            else if (iterations <= 0)
            {
                Console.WriteLine($"Please enter a number greater than 0.");
            }
        }
        while (iterations <= 0);

        deck = CreateDeck();
        dealtLowestCard = new int[numCardsDealt];

        for (int i = 0; i < iterations; i++)
        {
            if (i > 0)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

            progress = ((float)(i + 1) / (float)iterations) * 100.0f;
            Console.WriteLine($"Iteration: {i+1}/{iterations} ({progress.ToString("n2")}%)");
            DealAndCompare(numCardsDealt);
        }

        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.WriteLine($"For {numCardsDealt} cards dealt, over {iterations}, the Nth card had the lowest value the following number of times:");

        for (int i = 0; i < dealtLowestCard.Length; i++)
        {
            Console.WriteLine($"\t{i + 1}{NumberSuffix(i + 1)}: {dealtLowestCard[i]}.");
        }

        Console.WriteLine($"Press any key to exit...");
        Console.ReadLine();
    }

    private List<Card> CreateDeck()
    {
        List<Card> deck = new List<Card>();

        for (int i = 1; i <= (int)Suit.Hearts; i++)
        {
            for (int j = 1; j <= (int)Value.King; j++)
            {
                deck.Add(new Card((Suit)i, (Value)j));
            }
        }

        return deck;
    }

    private void DealAndCompare(int numCardsDealt)
    {
        deck.Shuffle();

        List<Card> cardsDealt = new List<Card>(deck.GetRange(0, numCardsDealt));
        deck.RemoveRange(0, numCardsDealt);

        int lowestCardIndex = 0;
        //Console.WriteLine($"Cards dealt:");
        //Console.WriteLine($"\t{cardsDealt[0].Name}");

        for (int i = 1; i < numCardsDealt; i++)
        {
            //Console.WriteLine($"\t{cardsDealt[i].Name}");
            if (!IsOtherCardHigher(cardsDealt[lowestCardIndex], cardsDealt[i]))
            {
                lowestCardIndex = i;
            }
        }

        //Console.WriteLine($"Lowest card: {cardsDealt[lowestCardIndex].Name}, at index {lowestCardIndex}");
        dealtLowestCard[lowestCardIndex]++;
        deck.AddRange(cardsDealt);
    }

    private bool IsOtherCardHigher(Card card, Card otherCard)
    {
        if (card.Value != otherCard.Value)
        {
            return otherCard.Value > card.Value;
        }

        return otherCard.Suit > card.Suit;
    }
}

