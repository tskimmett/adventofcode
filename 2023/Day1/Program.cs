using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

static class Program
{
	static void Main(string[] args)
	{
		if (args.FirstOrDefault() == "benchmark")
			BenchmarkRunner.Run<Solution>();
		else
		{
			var sln = new Solution(silent: false);
			sln.Part1();
			sln.Part2();
		}
	}
}

[MemoryDiagnoser]
public partial class Solution
{
	const string Digits = "[1-9]";
	const string DigitsAndNames = "[1-9]|one|two|three|four|five|six|seven|eight|nine";
	bool silent;

	public Solution(bool silent = true)
	{
		this.silent = silent;
	}

	[Benchmark]
	public void Part1() => Run(false);

	[Benchmark]
	public void Part2() => Run(true);

	void Run(bool isPart2)
	{
		var sum = 0;
		var firstNumRegex = isPart2 ? FirstNumRegex() : FirstDigitRegex();
		var lastNumRegex = isPart2 ? LastNumRegex() : LastDigitRegex();

		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day1/input.txt"));
		while (!file.EndOfStream)
		{
			var line = file.ReadLine()!;

			char firstDigit = NormalizeDigit(firstNumRegex.Match(line).ValueSpan);
			char lastDigit = NormalizeDigit(lastNumRegex.Match(line).ValueSpan);

			var calibrationValue = int.Parse(new ReadOnlySpan<char>([firstDigit, lastDigit]));

			sum += calibrationValue;
		}

		if (!silent)
			Console.WriteLine($"Sum of calibration values{(isPart2 ? " (pt2)" : "")} = {sum}");

		static char NormalizeDigit(ReadOnlySpan<char> text)
		{
			if (text.Length == 1)
			{
				return text[0];
			}
			else
			{
				return text switch
				{
					"one" => '1',
					"two" => '2',
					"three" => '3',
					"four" => '4',
					"five" => '5',
					"six" => '6',
					"seven" => '7',
					"eight" => '8',
					"nine" => '9',
					_ => throw new ArgumentException("Not a valid number", nameof(text))
				};
			}
		}
	}

	[GeneratedRegex(Digits, RegexOptions.Compiled)]
	private static partial Regex FirstDigitRegex();

	[GeneratedRegex(Digits, RegexOptions.Compiled | RegexOptions.RightToLeft)]
	private static partial Regex LastDigitRegex();

	[GeneratedRegex(DigitsAndNames, RegexOptions.Compiled)]
	private static partial Regex FirstNumRegex();

	[GeneratedRegex(DigitsAndNames, RegexOptions.Compiled | RegexOptions.RightToLeft)]
	private static partial Regex LastNumRegex();
}