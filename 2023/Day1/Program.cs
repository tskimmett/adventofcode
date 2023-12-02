using System.Text.RegularExpressions;

partial class Program
{
	const string Digits = "[1-9]";
	const string DigitsAndNames = "[1-9]|one|two|three|four|five|six|seven|eight|nine";

	static void Main(string[] args)
	{
		var p2 = args.FirstOrDefault() == "pt2";
		var sum = 0;
		var firstNumRegex = p2 ? FirstNumRegex() : FirstDigitRegex();
		var lastNumRegex = p2 ? LastNumRegex() : LastDigitRegex();

		using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day1/input.txt"));
		while (!file.EndOfStream)
		{
			var line = file.ReadLine()!;

			char firstDigit = NormalizeDigit(firstNumRegex.Match(line).ValueSpan);
			char lastDigit = NormalizeDigit(lastNumRegex.Match(line).ValueSpan);

			var calibrationValue = int.Parse(new ReadOnlySpan<char>([firstDigit, lastDigit]));

			sum += calibrationValue;
		}

		Console.WriteLine($"Sum of calibration values{(p2 ? " (pt2)" : "")} = {sum}");

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