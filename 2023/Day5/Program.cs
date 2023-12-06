using System.Buffers;

var digits = SearchValues.Create("0123456789");
var maps = new List<Map>();

using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day5/input.txt"));
var seeds = file.ReadLine()![7..].Split(' ').Select(long.Parse).ToArray();
var seedRanges = seeds
	.Select((val, idx) => (val, idx))
	.GroupBy(pair => pair.idx / 2)   // read seed numbers as groups of two (rangeStart, rangeSize)
	.Select(group => new LongRange(group.First().val, group.Last().val));

while (!file.EndOfStream)
{
	var line = file.ReadLine().AsSpan();

	if (line.Trim().Length == 0)
		continue;
	else if (digits.Contains(line[0]))
		maps.Last().AddRange(line.ToString());
	else
		maps.Add(new());
}

var lowestLocationForSeeds = seeds
	.Select(seed => maps.Aggregate(seed, (long source, Map map) => map.GetDestination(source)))
	.Min();

var lowestLocationForSeedRanges = seedRanges
	.Select(seedRange => maps.Aggregate(seedRange, (LongRange sourceRange, Map map) => map.GetDestinationRange(sourceRange)))
	.Select(range => range.Start)
	.Min();

Console.WriteLine($"(pt 1) Lowest location based on provided seeds: {lowestLocationForSeeds}");
Console.WriteLine($"(pt 2) Lowest location based on provided seed ranges: {lowestLocationForSeedRanges}");

public class Map()
{
	List<(LongRange sources, LongRange destinations)> Ranges { get; } = [];

	public void AddRange(string data)
	{
		var parts = data.Split(' ').Select(long.Parse).ToArray();
		var destStart = parts[0];
		var sourceStart = parts[1];
		var rangeSize = parts[2];
		Ranges.Add((new(sourceStart, rangeSize), new(destStart, rangeSize)));
	}

	public long GetDestination(long source)
	{
		foreach (var (sources, destinations) in Ranges)
		{
			if (sources.Contains(source))
				return destinations.Start + (source - sources.Start);
		}
		return source;
	}

	public LongRange GetDestinationRange(LongRange source)
	{
		foreach (var (sources, destinations) in Ranges)
		{
			if (sources.GetIntersection(source) is LongRange intersection)
			{
				var offset = intersection.Start - sources.Start;
				return new(destinations.Start + offset, intersection.Size);
			}
		}
		return source;
	}
}

public readonly record struct LongRange(long Start, long Size)
{
	long End { get; } = Start + Size - 1;

	public bool Contains(long value)
	{
		return Start <= value && End >= value;
	}

	public LongRange? GetIntersection(LongRange other)
	{
		var start = Math.Max(Start, other.Start);
		var end = Math.Min(End, other.End);
		if (start < end)
			return new(start, Size: end - start + 1);
		else
			return null;
	}
}