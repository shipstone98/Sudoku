namespace Sudoku
{
	/// <summary>
	/// Specifies constants that define difficulties for a <see cref="SudokuPuzzle"/>.
	/// </summary>
	public enum SudokuDifficulty
	{
		/// <summary>
		/// No specified difficulty. This is used when no difficulty information is available, e.g. when reading puzzles from files.
		/// </summary>
		None,

		/// <summary>
		/// An easy, low difficulty <see cref="SudokuPuzzle"/>.
		/// </summary>
		Easy,

		/// <summary>
		/// A medium difficulty <see cref="SudokuPuzzle"/>.
		/// </summary>
		Medium,

		/// <summary>
		/// A hard, high difficulty <see cref="SudokuPuzzle"/>.
		/// </summary>
		Hard
	}
}
