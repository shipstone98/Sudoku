using System;

namespace Sudoku
{
	/// <summary>
	/// Represents a method that handles the <see cref="Sudoku.Changed"/> event of a <see cref="Sudoku"/> puzzle.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The event data.</param>
	public delegate void SudokuChangedEventHandler(Object sender, SudokuEventArgs e);
}
