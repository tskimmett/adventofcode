
public class RockPaperScissors
{
	readonly (Outcome Player1, Outcome Player2) _outcome;
	readonly Shape _player1, _player2;

	public RockPaperScissors(Shape player1, Shape player2)
	{
		_player1 = player1;
		_player2 = player2;

		_outcome = Fight(player1, player2);
	}

	public RockPaperScissors(Shape player1, Outcome outcome2)
	{
		_player1 = player1;
		_outcome.Player2 = outcome2;
		(_outcome.Player1, _player2) = ReverseFight(player1, outcome2);
	}

	public (ushort Player1, ushort Player2) Scores
		=> (GetScore(_outcome.Player1, _player1), GetScore(_outcome.Player2, _player2));

	public static RockPaperScissors Parse(ReadOnlySpan<char> s) => new(ReadShape(s[0]), ReadShape(s[2..][0]));
	public static RockPaperScissors Parse2(ReadOnlySpan<char> s) => new(ReadShape(s[0]), ReadOutcome(s[2..][0]));

	static ushort GetScore(Outcome outcome, Shape shape) => (ushort)((ushort)outcome + (ushort)shape);

	(Outcome a, Outcome b) Fight(Shape a, Shape b) => a switch
	{
		_ when a == b => (Outcome.Draw, Outcome.Draw),
		Shape.Rock when b is Shape.Scissors => (Outcome.Win, Outcome.Lose),
		Shape.Rock when b is Shape.Paper => (Outcome.Lose, Outcome.Win),
		Shape.Paper when b is Shape.Rock => (Outcome.Win, Outcome.Lose),
		Shape.Paper when b is Shape.Scissors => (Outcome.Lose, Outcome.Win),
		Shape.Scissors when b is Shape.Paper => (Outcome.Win, Outcome.Lose),
		Shape.Scissors when b is Shape.Rock => (Outcome.Lose, Outcome.Win),
		_ => throw new ArgumentException($"Unhandled scenario: {a} versus {b}")
	};

	(Outcome a, Shape b) ReverseFight(Shape a, Outcome b) => b switch
	{
		Outcome.Draw => (Outcome.Draw, a),
		Outcome.Win when a is Shape.Rock => (Outcome.Lose, Shape.Paper),
		Outcome.Lose when a is Shape.Rock => (Outcome.Lose, Shape.Scissors),
		Outcome.Win when a is Shape.Paper => (Outcome.Lose, Shape.Scissors),
		Outcome.Lose when a is Shape.Paper => (Outcome.Lose, Shape.Rock),
		Outcome.Win when a is Shape.Scissors => (Outcome.Lose, Shape.Rock),
		Outcome.Lose when a is Shape.Scissors => (Outcome.Lose, Shape.Paper),
		_ => throw new ArgumentException($"Unhandled scenario: {b} versus {a}")
	};

	public enum Outcome : ushort
	{
		Lose = 0,
		Draw = 3,
		Win = 6
	}

	public enum Shape : ushort
	{
		Rock = 1,
		Paper = 2,
		Scissors = 3
	}

	static Shape ReadShape(char c)
	{
		return c switch
		{
			'A' or 'X' => Shape.Rock,
			'B' or 'Y' => Shape.Paper,
			'C' or 'Z' => Shape.Scissors,
			_ => throw new ArgumentException($"{c} is not a valid shape.")
		};
	}

	static Outcome ReadOutcome(char o)
	{
		return o switch
		{
			'A' or 'X' => Outcome.Lose,
			'B' or 'Y' => Outcome.Draw,
			'C' or 'Z' => Outcome.Win,
			_ => throw new ArgumentException($"{o} is not a valid outcome.")
		};
	}

}