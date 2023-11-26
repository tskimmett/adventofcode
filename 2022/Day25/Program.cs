(char carry, char sum) SnafuAdd(char a, char b)
{
	return (a + b) switch
	{
		'0' + '0' => ('0', '0'),
		'0' + '1' => ('0', '1'),
		'1' + '1' => ('0', '2'),
		'2' + '1' => ('1', '='),
		'2' + '2' => ('1', '-'),
		'0' + '-' => ('0', '-'),
		'1' + '-' => ('0', '0'),
		'2' + '-' => ('0', '1'),
		'0' + '=' => ('0', '='),
		'1' + '=' => ('0', '-'),
		'2' + '=' => ('0', '0'),
		'-' + '-' => ('0', '='),
		'-' + '=' => ('-', '2'),
		'=' + '=' => ('-', '1'),
		_ => throw new ArgumentException($"Either '{a}' or '{b}' is not a valid snafu digit.")
	};
}

// 1's place will be at 0 index
var finalSum = new char[64].Select(c => '0').ToArray();
var addends = new Stack<char[]>();

using var reader = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2022/Day25/input.txt"));
while (!reader.EndOfStream)
{
	var input = reader.ReadLine().AsSpan();

	// process input from right to left
	for (int j = 0, i = input.Length - 1; i >= 0; i--, j++)
	{
		var addend = input[i];
		var resultPlace = j;
		while (addend is not '0')
		{
			var (carry, sum) = SnafuAdd(addend, finalSum[resultPlace]);
			finalSum[resultPlace++] = sum;
			addend = carry;
		}
	}
}

var finalSumStr = string.Join(null, finalSum.Reverse()).TrimStart('0');
Console.WriteLine(finalSumStr);