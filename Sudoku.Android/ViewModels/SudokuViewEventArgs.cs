using System;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewEventArgs : EventArgs, ISudokuEventArgs
	{
		public int Column { get; private set; }
		public int Row { get; private set; }

		public SudokuViewEventArgs(int row, int column)
		{
			this.Row = row;
			this.Column = column;
		}
	}
}