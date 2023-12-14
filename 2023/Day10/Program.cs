internal class Program
{
	const string bends = "LF7J";
	const string reverseBends = "J7FL";
	static (int x, int y) bounds;

	static void Main()
	{
		Coord start = new(0, 0);
		List<char[]> grid = [];
		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day10/input.txt"));
		while (!file.EndOfStream)
		{
			var line = file.ReadLine()!;
			var startIndex = line.IndexOf('S');
			if (startIndex > -1)
				start = (startIndex, grid.Count);
			grid.Add(line.ToCharArray());
		}

		bounds = (x: grid[0].Length - 1, y: grid.Count - 1);
		// replace S with actual symbol
		grid[start.y][start.x] = GetStartSymbol(start, grid);

		HashSet<Coord> pipe = [];

		var steps = 0;
		var currPos = start;
		var prevPos = start;
		do
		{
			var currSymbol = Read(currPos, grid)!.Value;
			pipe.Add(currPos);
			var nextPos = WalkPipe(currSymbol, currPos, prevPos);
			prevPos = currPos;
			currPos = nextPos;
			steps++;
		} while (currPos != start);

		Console.WriteLine($"(pt 1) Steps to furthest point = {Math.Round(steps / 2f)}");

		HashSet<Coord> inside = [];
		HashSet<Coord> outside = [];

		for (var y = 0; y < grid.Count; y++)
		{
			var row = grid[y];
			Coord? lastPos = null;
			for (var x = 0; x < row.Length; x++)
			{
				Coord pos = new(x, y);
				if (!pipe.Contains(pos) && !inside.Contains(pos) && !outside.Contains(pos))
				{
					if (lastPos.HasValue && outside.Contains(lastPos.Value))
						outside.Add(pos);
					else if (CanReachBorder(pos))
						outside.Add(pos);
					else
						inside.Add(pos);
				}
				lastPos = pos;
			}
		}

		PrintGrid();
		Console.WriteLine($"(pt 2) Number of tiles enclosed by loop = {inside.Count}");

		void PrintGrid()
		{
			foreach (var (x, y) in inside)
				grid[y][x] = 'I';
			foreach (var (x, y) in outside)
				grid[y][x] = 'O';
			foreach (var row in grid)
				Console.WriteLine(new string(row));
		}

		bool IsOutside(bool wasOutside, char lastBend, char currBend)
		{
			var bendsDiff = bends.IndexOf(currBend) - bends.IndexOf(lastBend);
			if (bendsDiff == 1 || bendsDiff == -3)
				return wasOutside;

			bendsDiff = reverseBends.IndexOf(currBend) - reverseBends.IndexOf(lastBend);
			if (bendsDiff == 1 || bendsDiff == -3)
				return wasOutside;

			return !wasOutside;
		}

		bool CanReachBorder(Coord orig)
		{
			(Coord transform, char[] symbols)[] entrances = [
				((-1,0),  ['7','J']),
				((1,0),  ['F','L']),
				((0,1), ['7','F']),
				((0,-1), ['J','L'])
			];

			Coord? next = orig;
			HashSet<Coord> visited = [];
			do
			{
				Coord pos = next.Value;
				visited.Add(pos);
				next = null;

				if (IsBorder(pos) || outside.Contains(pos))
				{
					foreach (var v in visited)
						outside.Add(v);
					return true;
				}
				else if (inside.Contains(pos))
				{
					foreach (var v in visited)
						inside.Add(v);
					return false;
				}

				// squeeze between pipe
				foreach (var (t, symbols) in entrances)
				{
					var neighbor = pos + t;
					if (visited.Contains(neighbor))
						continue;

					if (Read(neighbor, grid) is char symbol)
					{
						if (!pipe.Contains(neighbor))
							next = neighbor;
						else if (symbols.Contains(symbol))
							return IsOutsidePipeLoop(neighbor, t, symbol);
					}
				}
			} while (next != null);
			return false;

			bool IsOutsidePipeLoop(Coord pipeEntrance, Coord firstStep, char entranceBend)
			{
				var isOutside = true;
				var insideCount = 0;
				var outsideCount = 1;
				var lastBend = entranceBend;
				var currPos = pipeEntrance + firstStep;
				var prevPos = pipeEntrance;
				do
				{
					var currSymbol = Read(currPos, grid)!.Value;

					if (bends.Contains(currSymbol))
					{
						isOutside = IsOutside(isOutside, lastBend, currSymbol);
						if (isOutside)
							outsideCount++;
						else
							insideCount++;
						lastBend = currSymbol;
					}

					var nextPos = WalkPipe(currSymbol, currPos, prevPos);
					prevPos = currPos;
					currPos = nextPos;
				} while (currPos != pipeEntrance);

				return outsideCount > insideCount;
			}
		}

		bool IsBorder(Coord pos)
		{
			var (x, y) = pos;
			return x == 0 || x == bounds.x || y == 0 || y == bounds.y;
		}


		Coord WalkPipe(char currentSymbol, Coord currentPos, Coord previousPos)
		{
			(Coord a, Coord b) = currentSymbol switch
			{
				'|' => ((0, 1), (0, -1)),
				'-' => ((1, 0), (-1, 0)),
				'F' => ((1, 0), (0, 1)),
				'L' => ((1, 0), (0, -1)),
				'J' => ((-1, 0), (0, -1)),
				'7' => ((-1, 0), (0, 1)),
				_ => throw new($"{currentSymbol} is not a valid pipe segment")
			};

			var result = currentPos + a;
			return result != previousPos
				 ? result
				 : currentPos + b;
		}
	}

	static char? Read(Coord n, List<char[]> grid)
	{
		var (x, y) = n;
		if (x >= 0 && y >= 0 && x <= bounds.x && y <= bounds.y)
		{
			var row = grid[y];
			return x < row.Length ? row[x] : null;
		}
		return null;
	}

	static char GetStartSymbol(Coord start, List<char[]> grid)
	{
		List<Coord> starts = [];
		(Coord transform, char[] symbols)[] transforms = [
			((0,-1),  ['|', '7', 'F']),
			((1,0),  ['-', 'J', '7']),
			((0,1), ['|', 'L','J']),
			((-1,0), ['-', 'F', 'L'])
		];

		foreach (var (t, symbols) in transforms)
		{
			var neighbor = start + t;
			if (Read(neighbor, grid) is char symbol && symbols.Contains(symbol))
				starts.Add(neighbor);
		}

		return GetSymbolByTransform(start - starts[1], starts[0] - start);
	}

	static char GetSymbolByTransform(Coord step1, Coord step2)
	{
		return (step1, step2) switch
		{
			((0, 1), (0, 1)) => '|',
			((1, 0), (1, 0)) => '-',
			((1, 0), (0, 1)) => '7',
			((-1, 0), (0, 1)) => 'F',
			((-1, 0), (0, -1)) => 'L',
			((1, 0), (0, -1)) => 'J',
			_ => GetSymbolByTransform(-step2, -step1)
		};
	}
}

internal record struct Coord(int x, int y)
{
	public static implicit operator (int x, int y)(Coord value)
		=> (value.x, value.y);

	public static implicit operator Coord((int x, int y) value)
		=> new Coord(value.x, value.y);

	public static Coord operator +(Coord a, Coord b)
		=> new(a.x + b.x, a.y + b.y);

	public static Coord operator -(Coord a, Coord b)
		=> new(a.x - b.x, a.y - b.y);

	public static Coord operator -(Coord a)
		=> new(-a.x, -a.y);
}