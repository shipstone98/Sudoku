using System;

namespace Sudoku
{
	/// <summary>
	/// Provides data for when a <see cref="Sudoku"/> puzzle event is raised.
	/// </summary>
	public class SudokuEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the zero-based column of the <see cref="Sudoku"/> puzzle where the event was raised.
		/// </summary>
		public int Column { get; }

		/// <summary>
		/// Gets the number that raised the event.
		/// </summary>
		public int Number { get; }

		/// <summary>
		/// Gets the zero-based row of the <see cref="Sudoku"/> puzzle where the event was raised.
		/// </summary>
		public int Row { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuEventArgs"/> class, specifying the <c><paramref name="row"/></c> and <c><paramref name="column"/></c> where the event was raised and the <c><paramref name="number"/></c> that raised the event.
		/// </summary>
		/// <param name="row">The zero-based row of the <see cref="Sudoku"/> puzzle where the event was raised.</param>
		/// <param name="column">The zero-based column of the <see cref="Sudoku"/> puzzle where the event was raised.</param>
		/// <param name="number">The <c><paramref name="number"/></c> that raised the event.</param>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c>, <c><paramref name="column"/></c> or <c><paramref name="number"/></c> are less than 0.</exception>
		public SudokuEventArgs(int row, int column, int number)
		{
			if (row < 0)
			{
				throw new ArgumentOutOfRangeException(nameof (row));
			}

			if (column < 0)
			{
				throw new ArgumentOutOfRangeException(nameof (column));
			}

			if (number < 0)
			{
				throw new ArgumentOutOfRangeException(nameof (number));
			}

			this.Row = row;
			this.Column = column;
			this.Number = number;
		}
	}
}
