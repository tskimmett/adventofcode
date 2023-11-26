public partial class Program
{
	public static void Part2()
	{
		const int topN = 3;
		var maxCalories = new LinkedList<int>();
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

		Console.WriteLine($"Sum of top {topN}: {maxCalories.Sum()}");

		void FinalizeElf()
		{
			if (maxCalories.Count == 0)
			{
				maxCalories.AddFirst(currCalories);
			}
			else
			{
				var node = maxCalories.First;
				while (node is not null)
				{
					if (currCalories > node.Value)
					{
						maxCalories.AddBefore(node, currCalories);
						break;
					}
					node = node.Next;
				}
			}

			if (maxCalories.Count > topN)
				maxCalories.RemoveLast();

			currCalories = 0;
		}
	}
}