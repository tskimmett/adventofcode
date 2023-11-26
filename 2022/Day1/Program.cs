
public partial class Program
{
	private static void Main(string[] args)
	{
		if (args.FirstOrDefault() == "p2")
			Part2();
		else
			Part1();
	}
}