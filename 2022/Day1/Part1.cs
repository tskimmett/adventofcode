public partial class Program
{
	public static void Part1()
	{
		var maxCalories = 0;
		var currCalories = 0;

		using var reader = new StreamReader(File.OpenRead("/Users/tkimmett/source/adventofcode/2022/Day1/input.txt"));
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()?.Trim();

			if (string.IsNullOrWhiteSpace(line))
				FinalizeElf();
			else
				currCalories += int.Parse(line);
		}

		Console.WriteLine($"maxCalories = {maxCalories}");

		void FinalizeElf()
		{
			if (currCalories > maxCalories)
				maxCalories = currCalories;
			currCalories = 0;
		}
	}
}