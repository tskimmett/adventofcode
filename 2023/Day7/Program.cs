List<Hand> hands = new();
using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day7/input.txt"));
while (!file.EndOfStream)
{
	var line = file.ReadLine().AsSpan();
	var cards = line[..5].ToString().Select(c => Enum.Parse<Card>(new ReadOnlySpan<char>(ref c)));
	var bid = int.Parse(line[6..].Trim());
	hands.Add(new(cards, bid));
}

var totalWinnings = hands.Order()
	.Select((hand, idx) => (hand, rank: idx + 1))
	.Aggregate(0, (sum, i) => sum + i.hand.Bid * i.rank);

var totalJokerWinnings = hands.Order(new JokerComparer())
	.Select((hand, idx) => (hand, rank: idx + 1))
	.Aggregate(0, (sum, i) => sum + i.hand.Bid * i.rank);

Console.WriteLine($"(pt 1) Total winnings = {totalWinnings}");
Console.WriteLine($"(pt 2) Total winnings with jokers = {totalJokerWinnings}");


public readonly record struct Hand : IComparable<Hand>
{
	public IReadOnlyList<Card> Cards { get; }
	public HandType Type { get; }
	public HandType JokerType { get; }
	public int Bid { get; }

	readonly string cardString;

	public override string ToString() => cardString;

	public Hand(IEnumerable<Card> cards, int bid)
	{
		Cards = cards.ToList();
		Type = GetHandType();
		JokerType = GetJokerHandType();
		Bid = bid;
		cardString = string.Join("", Cards).Replace("_", "");
	}

	HandType GetJokerHandType()
	{
		var groups = Cards
			.GroupBy(c => c)
			.Select(g => (card: g.First(), count: g.Count()))
			.Where(g => g.card != Card.J)
			.OrderByDescending(g => g.count)
			.ThenByDescending(g => g.card)
			.Select(g => g.card)
			.ToList();

		var bestCard = groups.FirstOrDefault(Card.J);
		var withReplacements = Cards.Select(c => c == Card.J ? bestCard : c);
		return GetHandType(withReplacements);
	}

	HandType GetHandType() => GetHandType(Cards);

	static HandType GetHandType(IEnumerable<Card> cards)
	{
		var groups = cards.GroupBy(c => c).ToList();
		var topGroup = groups.Max(g => g.Count());
		return topGroup switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			1 => HandType.HighCard,
			3 when groups.Count == 2 => HandType.FullHouse,
			3 when groups.Count == 3 => HandType.ThreeOfAKind,
			2 when groups.Count == 3 => HandType.TwoPair,
			2 when groups.Count == 4 => HandType.OnePair,
			_ => throw new($"Unknown hand {{ {string.Join(", ", cards)} }}")
		};
	}

	public int CompareTo(Hand other) => CompareTo(other, useJokers: false);
	public int CompareTo(Hand other, bool useJokers)
	{
		var type = useJokers ? JokerType : Type;
		var otherType = useJokers ? other.JokerType : other.Type;
		var typeCompare = type.CompareTo(otherType);
		if (typeCompare != 0)
			return typeCompare;
		else
		{
			for (var i = 0; i < Cards.Count; i++)
			{
				var mine = Cards[i];
				var theirs = other.Cards[i];
				if (mine != theirs)
					return CardValue(mine, useJokers) - CardValue(theirs, useJokers);
			}
			return 0;
		}
	}

	static int CardValue(Card card, bool useJokers)
	  => useJokers && card == Card.J ? 1 : (int)card;
}

public class JokerComparer : IComparer<Hand>
{
	public int Compare(Hand x, Hand y) => x.CompareTo(y, useJokers: true);
}


public enum HandType
{
	HighCard,
	OnePair,
	TwoPair,
	ThreeOfAKind,
	FullHouse,
	FourOfAKind,
	FiveOfAKind
}

public enum Card
{
	_2 = 2,
	_3 = 3,
	_4 = 4,
	_5 = 5,
	_6 = 6,
	_7 = 7,
	_8 = 8,
	_9 = 9,
	T = 10,
	J = 11,
	Q = 12,
	K = 13,
	A = 14
}