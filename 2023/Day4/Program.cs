internal class Program
{
	const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

	static void Main()
	{
		var totalPoints = 0;
		var cardCounts = new Dictionary<int, int>();
		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day4/input.txt"));
		while (!file.EndOfStream)
		{
			var card = new Card(file.ReadLine()!);

			AddCard(card.Id, copies: 1);

			if (card.IsWinner)
			{
				totalPoints += card.Points;

				foreach (var id in Enumerable.Range(card.Id + 1, card.MatchCount))
					AddCard(id, copies: cardCounts[card.Id]);
			}
		}

		Console.WriteLine($"(pt1) Total points = {totalPoints}");
		Console.WriteLine($"(pt2) Total cards with copying = {cardCounts.Values.Sum()}");

		void AddCard(int id, int copies)
		{
			if (!cardCounts.TryAdd(id, copies))
				cardCounts[id] += copies;
		}
	}

	public readonly record struct Card
	{
		public int Id { get; }
		public int MatchCount { get; }
		public int Points { get; }
		public bool IsWinner => MatchCount > 0;

		public Card(string input)
		{
			var headerSplit = input.Split(":", splitOptions);

			Id = int.Parse(headerSplit[0].AsSpan()[5..]);

			var cardData = headerSplit[1].Split(" | ", splitOptions);

			var winningNums = cardData[0].Split(' ', splitOptions).ToHashSet();
			var myNums = cardData[1].Split(' ', splitOptions).ToHashSet();

			MatchCount = winningNums.Intersect(myNums).Count();
			Points = CalculatePoints();
		}

		int CalculatePoints()
			 => MatchCount > 0 ? (int)Math.Pow(2, MatchCount - 1) : 0;
	}
}