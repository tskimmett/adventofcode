using System.Buffers;

public class Program
{
	const int MAX_SETS_PER_LINE = 20;
	static void Main(string[] args)
	{
		var sumPossibleGameIds = 0;
		var sumMinSetPowers = 0;
		var constraints = new Cubes(Red: 12, Green: 13, Blue: 14);

		Span<Range> marbleSets = stackalloc Range[MAX_SETS_PER_LINE];
		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day2/input.txt"));
		while (!file.EndOfStream)
		{
			var line = file.ReadLine().AsSpan();
			var idxColon = line.IndexOf(':');
			var body = line[(idxColon + 1)..].Trim();

			var numSets = body.Split(marbleSets, ';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			var cubeSets = new List<Cubes>(numSets);
			var isPossible = true;

			foreach (var set in marbleSets[..numSets])
			{
				var cubes = Cubes.Parse(body[set]);
				if (!constraints.CanContain(cubes))
					isPossible = false;

				cubeSets.Add(cubes);
			}

			if (isPossible)
			{
				var header = line[..idxColon].Trim();
				var gameId = header[(header.IndexOf(' ') + 1)..];
				sumPossibleGameIds += int.Parse(gameId);
			}

			sumMinSetPowers += Cubes.MinimumSet(cubeSets).Power;
		}

		Console.WriteLine($"Given a bag of {constraints}, sum of possible game ids = {sumPossibleGameIds}");
		Console.WriteLine($"Sum of minimum cube sets = {sumMinSetPowers}");
	}

	record struct Cubes(int Red, int Green, int Blue)
	{
		static readonly SearchValues<char> digits = SearchValues.Create("0123456789");
		public readonly bool CanContain(Cubes other)
			=> Red >= other.Red && Green >= other.Green && Blue >= other.Blue;

		public readonly int Power => Red * Green * Blue;

		public static Cubes MinimumSet(IEnumerable<Cubes> cubes)
			=> new(
				cubes.Max(c => c.Red),
				cubes.Max(c => c.Green),
				cubes.Max(c => c.Blue)
			);

		public static Cubes Parse(ReadOnlySpan<char> input)
		{
			var marbles = new Cubes(0, 0, 0);
			var marbleData = new Range[10];
			var numCubes = input.SplitAny(marbleData, ", ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			ReadOnlySpan<char> currentColor = default;
			for (var i = numCubes - 1; i >= 0; i--)
			{
				var token = input[marbleData[i]];
				var isColor = token.ContainsAnyExcept(digits);
				if (isColor)
					currentColor = token;
				else
					marbles.SetCount(currentColor, int.Parse(token));
			}
			return marbles;
		}

		void SetCount(ReadOnlySpan<char> color, int count)
		{
			if (color is "red")
				Red = count;
			else if (color is "green")
				Green = count;
			else if (color is "blue")
				Blue = count;
			else
				throw new ArgumentException($"{color} is not a valid marble color");
		}
	}
}