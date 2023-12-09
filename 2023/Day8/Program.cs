Graph graph = new();

using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day8/input.txt"));
var instructions = file.ReadLine()!;
file.ReadLine();
while (!file.EndOfStream)
{
	var line = file.ReadLine().AsSpan();
	var name = line[..3].ToString();
	var left = line.Slice(7, 3).Trim().ToString();
	var right = line.Slice(12, 3).Trim().ToString();

	graph.RegisterNode(name, left, right);
}

var (starts, ends) = graph.Realize(isStart: name => name == "AAA", isEnd: name => name == "ZZZ");
var stepsToZZZ = StepsFromManyToMany(instructions, starts, ends);
Console.WriteLine($"(pt 1) Steps from AAA to ZZZ = {stepsToZZZ}");

(starts, ends) = graph.Realize(isStart: name => name[^1] == 'A', isEnd: name => name[^1] == 'Z');
var stepsToEndsWithZ = StepsFromManyToMany(instructions, starts, ends);
Console.WriteLine($"(pt 2) Steps from all A nodes to all Z nodes = {stepsToEndsWithZ}");

ulong StepsFromManyToMany(string instructions, ISet<INode> startSet, ISet<INode> endSet)
{
	var starts = startSet.ToArray();
	var ends = endSet.ToArray();
	var stepCounts = starts
		.Select(start => StepsToEnd(instructions, start, endSet))
		.ToList();

	return stepCounts.Count == 1 ? stepCounts.First() : LcmMany(stepCounts);
}

ulong StepsToEnd(string instructions, INode start, ISet<INode> endSet)
{
	var steps = 0u;
	var pos = start;
	while (true)
	{
		foreach (var dir in instructions)
		{
			pos = dir is 'R' ? pos.Right : pos.Left;
			++steps;
			if (endSet.Contains(pos))
				return steps;
		}
	}
}

ulong LcmMany(IEnumerable<ulong> args)
{
	if (args.Count() == 2)
		return Lcm(args.First(), args.Last());
	else
		return LcmMany([Lcm(args.ElementAt(0), args.ElementAt(1)), .. args.Skip(2)]);
}

ulong Lcm(ulong a, ulong b)
	=> a * b / Gcd(a, b);

ulong Gcd(ulong a, ulong b)
{
	if (b == 0)
		return a;
	var max = Math.Max(a, b);
	var min = Math.Min(a, b);
	return Gcd(min, max % min);
}

public class Graph
{
	readonly Dictionary<string, Node> nodes = [];

	public void RegisterNode(string name, string left, string right)
	{
		if (!nodes.ContainsKey(name))
			nodes[name] = new Node { Name = name, LeftName = left, RightName = right };
	}

	public (ISet<INode> starts, ISet<INode> ends) Realize(Func<string, bool> isStart, Func<string, bool> isEnd)
	{
		HashSet<INode> starts = [];
		HashSet<INode> ends = [];
		foreach (var (name, node) in nodes)
		{
			node.Left ??= nodes[node.LeftName];
			node.Right ??= nodes[node.RightName];

			if (isStart(name))
				starts.Add(node);
			if (isEnd(name))
				ends.Add(node);
		}

		return (starts, ends);
	}

	class Node : INode
	{
		public required string Name { get; init; }
		public required string LeftName { get; init; }
		public required string RightName { get; init; }

		public Node? Left { get; set; }
		public Node? Right { get; set; }

		public override string ToString() => $"{Name} {{{Left?.Name} | {Right?.Name}}}";

		INode INode.Left => Left!;
		INode INode.Right => Right!;
	}
}

public interface INode
{
	public string Name { get; }
	public INode Left { get; }
	public INode Right { get; }
}