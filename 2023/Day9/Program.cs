
List<History> histories = [];
using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day9/input.txt"));
while (!file.EndOfStream)
{
	histories.Add(new(file.ReadLine()!));
}

Console.WriteLine($"(pt 1) Sum of future extrapolated values = {histories.Sum(h => h.FutureExtrapolation)}");
Console.WriteLine($"(pt 2) Sum of past extrapolated values = {histories.Sum(h => h.PastExtrapolation)}");

public class History
{
	List<List<int>> sequences = [];
	public int FutureExtrapolation => sequences[0][^1];
	public int PastExtrapolation => sequences[0][0];
	public History(string input)
	{
		sequences.Add(input.Split(' ').Select(int.Parse).ToList());
		GenerateDifferences();
		Extrapolate();
	}

	void Extrapolate()
	{
		sequences.Last().Add(0);
		sequences.Last().Prepend(0);
		for (var i = sequences.Count - 2; i >= 0; i--)
		{
			var sequence = sequences[i];
			var lowerSequence = sequences[i + 1];
			sequence.Add(sequence[^1] + lowerSequence[^1]);
			sequence.Insert(0, sequence[0] - lowerSequence[0]);
		}
	}

	void GenerateDifferences()
	{
		List<int> sequence = sequences.Last();
		while (!sequence.All(n => n == 0))
		{
			List<int> diffs = [];
			for (var i = 1; i < sequence.Count; i++)
				diffs.Add(sequence[i] - sequence[i - 1]);
			sequences.Add(diffs);
			sequence = sequences.Last();
		}
	}
}