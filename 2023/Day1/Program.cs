using System.Text.RegularExpressions;

var sum = 0;
var numRegex = args.FirstOrDefault() == "p2"
	? NumRegex()
	: DigitRegex();

using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day1/input.txt"));
while (!file.EndOfStream)
{
	var line = file.ReadLine();
	var matches = numRegex.Matches(line!);

	var firstDigit = ToNumber(matches.First().Value);
	var lastDigit = ToNumber(matches.Last().Value, end: true);

	var calibrationValue = int.Parse(new ReadOnlySpan<char>([firstDigit, lastDigit]));

	sum += calibrationValue;
}

Console.WriteLine($"Sum of calibration values = {sum}");

static char ToNumber(string value, bool end = false)
{
	if (value.Length == 1)
	{
		return value[0];
	}
	else
	{
		return value switch
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
			"twone" when end => '1',
			"twone" when !end => '2',
			"sevenine" when end => '9',
			"sevenine" when !end => '7',
			"threeight" when end => '8',
			"threeight" when !end => '3',
			"nineight" when end => '8',
			"nineight" when !end => '9',
			"fiveight" when end => '8',
			"fiveight" when !end => '5',
			"oneight" when end => '8',
			"oneight" when !end => '1',
			"eightwo" when end => '2',
			"eightwo" when !end => '8',
			_ => throw new ArgumentException("Not a valid number", nameof(value))
		};
	}
}

partial class Program
{
	[GeneratedRegex("[1-9]", RegexOptions.Compiled)]
	private static partial Regex DigitRegex();

	[GeneratedRegex("[1-9]|eightwo|oneight|fiveight|twone|sevenine|threeight|nineight|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled)]
	private static partial Regex NumRegex();
}