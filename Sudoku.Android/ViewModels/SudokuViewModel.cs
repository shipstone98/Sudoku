using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using AndroidX.Lifecycle;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewModel : ViewModel
	{
		private const int DefaultColumn = -1;
		private const int DefaultRow = -1;

		public int Column { get; private set; }
		public int Row { get; private set; }
		public Sudoku Sudoku { get; set; }

		public SudokuViewModel()
		{
			this.Column = SudokuViewModel.DefaultColumn;
			this.Row = SudokuViewModel.DefaultRow;
			this.Sudoku = new Sudoku(9, SudokuDifficulty.Easy);
			SudokuGenerator.AddNumbers(this.Sudoku);
			SudokuGenerator.RemoveNumbers(this.Sudoku);
		}

		public void SetRowAndColumn(int row, int column)
		{
			if (row < 0 || column < 0 || row == this.Row && column == this.Column)
			{
				row = SudokuViewModel.DefaultRow;
				column = SudokuViewModel.DefaultColumn;
			}

			else if (!(this.Sudoku is null) && this.Sudoku.CheckReadOnly(row, column))
			{
				return;
			}

			this.Row = row;
			this.Column = column;
		}
	}
}