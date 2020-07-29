namespace Sudoku
{
	/// <summary>
	/// Provides data for when a <see cref="SudokuPuzzle"/> event is raised.
	/// </summary>
	public interface ISudokuEventArgs
	{
		/// <summary>
		/// Gets the zero-based column of the <see cref="SudokuPuzzle"/> where the event was raised.
		/// </summary>
		int Column { get; }

		/// <summary>
		/// Gets the zero-based row of the <see cref="SudokuPuzzle"/> where the event was raised.
		/// </summary>
		int Row { get; }
	}
}
