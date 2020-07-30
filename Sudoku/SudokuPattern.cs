namespace Sudoku
{
	/// <summary>
	/// Specified constants that define moves used to solve a <see cref="SudokuPuzzle"/>.
	/// </summary>
    public enum SudokuPattern
    {
		/// <summary>
		/// No specified pattern. This can be used when solving using recursion, rather than pattern matching.
		/// </summary>
        None,

		/// <summary>
		/// Defines a claiming locked candidate.
		/// </summary>
        ClaimingCandidate,

		/// <summary>
		/// Defines a hidden single.
		/// </summary>
        HiddenSingle,

		/// <summary>
		/// Defines a naked locked pair.
		/// </summary>
		NakedPair,

		/// <summary>
		/// Defines a naked single.
		/// </summary>
        NakedSingle,

		/// <summary>
		/// Defines a pointing locked candidate.
		/// </summary>
        PointingCandidate
    }
}