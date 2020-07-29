using System;

namespace Sudoku
{
	/// <summary>
	/// Represents a method that handles the <see cref="SudokuPuzzle.Changed"/> event of a <see cref="SudokuPuzzle"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The event data.</param>
	public delegate void SudokuChangedEventHandler(Object sender, SudokuEventArgs e);
}
