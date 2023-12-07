using System.Text.RegularExpressions;

var space = new Regex(@"\s+");
using var file = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2023/Day6/input.txt"));

var times = space.Split(file.ReadLine()!)[1..].Select(long.Parse);
var distances = space.Split(file.ReadLine()!)[1..].Select(long.Parse);
var races = times.Zip(distances);

long productOfPossibilities = 1;
foreach (var (time, distance) in races)
{
	long winningStrats = CountWinningStrategies(time, distance);
	productOfPossibilities *= winningStrats;
}

var bigTime = long.Parse(string.Join("", times));
var bigDist = long.Parse(string.Join("", distances));
var bigRacePossibilities = CountWinningStrategies(bigTime, bigDist);

Console.WriteLine($"(pt 1) Product of possible winning strategies = {productOfPossibilities}");
Console.WriteLine($"(pt 2) Possible winning strategies for big race = {bigRacePossibilities}");

static long CountWinningStrategies(long time, long distance)
{
	var winningStrats = 0;
	for (var v = 1; v < time; v++)
	{
		var d = v * (time - v);
		if (d > distance)
			winningStrats++;
	}

	return winningStrats;
}