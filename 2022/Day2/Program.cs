
internal partial class Program
{
	static void Main(string[] args)
	{
		var useAlternateParse = args.FirstOrDefault() == "p2";
		var totalScore = 0;
		using var reader = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2022/Day2/input.txt"));
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine().AsSpan();
			var round = useAlternateParse ? RockPaperScissors.Parse2(line) : RockPaperScissors.Parse(line);
			totalScore += round.Scores.Player2;
		}

		Console.WriteLine($"Total score: {totalScore}");
	}
}