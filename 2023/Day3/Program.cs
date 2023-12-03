using System.Buffers;

internal class Program
{
	static void Main()
	{
		var sumPartNumbers = 0;
		var sumGearRatios = 0;

		Line prevLine = default;
		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day3/input.txt"));
		while (!file.EndOfStream)
		{
			Line line = new(file.ReadLine().AsSpan());
			if (!prevLine.IsEmpty)
			{
				prevLine.CrossReference(line);
				Dispose(prevLine);
			}
			prevLine = line;
		}
		Dispose(prevLine);

		Console.WriteLine($"Sum of engine part numbers = {sumPartNumbers}");
		Console.WriteLine($"Sum of gear ratios = {sumGearRatios}");

		void Dispose(Line line)
		{
			sumPartNumbers += line.GetPartNumberSum();
			sumGearRatios += line.GetGearRatioSum();
		}
	}

	readonly ref struct Line
	{
		const string digits = "0123456789";
		static readonly SearchValues<char> Digits = SearchValues.Create(digits);
		static readonly SearchValues<char> NonSymbols = SearchValues.Create(digits + ".");

		readonly ReadOnlySpan<char> _line;

		List<Index> Symbols { get; } = [];
		List<Range> Numbers { get; } = [];
		List<int> PartNumbers { get; } = [];
		Dictionary<Index, List<int>> PotentialGears { get; } = [];

		public bool IsEmpty => _line.IsEmpty;

		public int GetPartNumberSum() => PartNumbers.Sum();

		public int GetGearRatioSum()
			=> PotentialGears.Values
				.Where(parts => parts.Count == 2)
				.Sum(parts => parts[0] * parts[1]);

		public void CrossReference(Line adjacentLine)
		{
			CheckPartNumbersAgainst(adjacentLine);
			adjacentLine.CheckPartNumbersAgainst(this);
		}

		public Line(ReadOnlySpan<char> line)
		{
			_line = line;
			ReadTokens();
			CheckPartNumbersAgainst(this);
		}

		static bool IsSymbol(char c) => !NonSymbols.Contains(c);
		static bool IsGear(char c) => c is '*';
		static bool IsDigit(char c) => Digits.Contains(c);
		static bool AreAdjacent(Index symbolIndex, Range numRange)
		{
			var left = numRange.Start.Value - 1;
			var right = numRange.End.Value;
			var s = symbolIndex.Value;
			return left <= s && s <= right;
		}

		void CheckPartNumbersAgainst(Line adjacentLine)
		{
			foreach (var symbolIndex in adjacentLine.Symbols)
			{
				var i = 0;
				while (i < Numbers.Count)
				{
					var numRange = Numbers[i];
					if (AreAdjacent(symbolIndex, numRange))
					{
						Numbers.RemoveAt(i);

						var partNum = int.Parse(_line[numRange]);
						PartNumbers.Add(partNum);

						if (adjacentLine.PotentialGears.TryGetValue(symbolIndex, out var gearParts))
							gearParts.Add(partNum);
					}
					else if (numRange.Start.Value - symbolIndex.Value > 0)
						break;   // once we reach a number that is too far right of the symbol, move on
					else
						i++;
				}
			}
		}

		void ReadTokens()
		{
			int? numStart = null;
			for (var i = 0; i < _line.Length; i++)
			{
				var chr = _line[i];
				var isDigit = Digits.Contains(chr);
				if (isDigit)
					numStart ??= i;


				if (numStart.HasValue)
				{
					int? numEnd = null;
					if (!isDigit)
						numEnd = i;
					else if (i == _line.Length - 1)
						numEnd = _line.Length;

					if (numEnd.HasValue)
					{
						Numbers.Add(new(numStart.Value, numEnd.Value));
						numStart = null;
					}
				}

				if (IsSymbol(chr))
				{
					Symbols.Add(i);
					if (IsGear(chr))
						PotentialGears.Add(i, []);
				}
			}
		}
	}
}