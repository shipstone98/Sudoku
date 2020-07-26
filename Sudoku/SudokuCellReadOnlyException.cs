using System;

namespace Sudoku
{
	/// <summary>
	/// The exception that is thrown when an attempt is made to modify the value of a read-only <see cref="Sudoku"/> puzzle cell. 
	/// </summary>
	public class SudokuCellReadOnlyException : Exception
	{
		/// <summary>
		/// The default exception message to be used when no message, or a null string message, is provided.
		/// </summary>
		public const String DefaultMessage = "An attempt was made to modify the value of a read-only Sudoku puzzle cell.";

		/// <summary>
		/// Represents a zero-based unspecified column in a <see cref="Sudoku"/> puzzle.
		/// </summary>
		public const int UnspecifiedColumn = -1;

		/// <summary>
		/// Represents a zero-based unspecified row in a <see cref="Sudoku"/> puzzle.
		/// </summary>
		public const int UnspecifiedRow = -1;

		/// <summary>
		/// Gets the zero-based column of the <see cref="Sudoku"/> puzzle where the exception was thrown.
		/// </summary>
		public int Column { get; }

		/// <summary>
		/// Gets the zero-based row of the <see cref="Sudoku"/> puzzle where the exception was thrown.
		/// </summary>
		public int Row { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with the default unspecified row and column values.
		/// </summary>
		public SudokuCellReadOnlyException() : this(SudokuCellReadOnlyException.UnspecifiedRow, SudokuCellReadOnlyException.UnspecifiedColumn) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with a specified error message and the default unspecified row and column values.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public SudokuCellReadOnlyException(String message) : this(message ?? SudokuCellReadOnlyException.DefaultMessage, SudokuCellReadOnlyException.UnspecifiedRow, SudokuCellReadOnlyException.UnspecifiedColumn) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception and the default unspecified row and column values.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception, or a null reference (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
		public SudokuCellReadOnlyException(String message, Exception inner) : this(message ?? SudokuCellReadOnlyException.DefaultMessage, inner, SudokuCellReadOnlyException.UnspecifiedRow, SudokuCellReadOnlyException.UnspecifiedColumn) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with specified <c><paramref name="row"/></c> and <c><paramref name="column"/></c> values.
		/// </summary>
		/// <param name="row">The row of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		/// <param name="column">The column of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		public SudokuCellReadOnlyException(int row, int column)
		{
			this.Column = column;
			this.Row = row;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with a specified error message and <c><paramref name="row"/></c> and <c><paramref name="column"/></c> values.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="row">The row of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		/// <param name="column">The column of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		public SudokuCellReadOnlyException(String message, int row, int column) : base(message ?? SudokuCellReadOnlyException.DefaultMessage)
		{
			this.Column = column;
			this.Row = row;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuCellReadOnlyException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception and <c><paramref name="row"/></c> and <c><paramref name="column"/></c> values.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="inner">The exception that is the cause of the current exception, or a null reference (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
		/// <param name="row">The row of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		/// <param name="column">The column of the <see cref="Sudoku"/> puzzle where the exception was thrown.</param>
		public SudokuCellReadOnlyException(String message, Exception inner, int row, int column) : base(message ?? SudokuCellReadOnlyException.DefaultMessage, inner)
		{
			this.Column = column;
			this.Row = row;
		}
	}
}
