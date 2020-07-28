using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
	/// <summary>
	/// Provides data for when a <see cref="Sudoku"/> puzzle event is raised.
	/// </summary>
	public interface ISudokuEventArgs
	{
		/// <summary>
		/// Gets the zero-based column of the <see cref="Sudoku"/> puzzle where the event was raised.
		/// </summary>
		int Column { get; }

		/// <summary>
		/// Gets the zero-based row of the <see cref="Sudoku"/> puzzle where the event was raised.
		/// </summary>
		int Row { get; }
	}
}
