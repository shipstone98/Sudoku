using System;

namespace Sudoku
{
	public class SudokuPossibleMove : SudokuMove
	{
		private readonly int[] _ColumnsAffected;
		private readonly int[] _NumbersAffected;
		private readonly int[] _RowsAfffected;

		public int[] ColumnsAffected => this._ColumnsAffected;
		public int[] NumbersAffected => this._NumbersAffected;
		public int[] RowsAffected => this._RowsAfffected;

		public SudokuPossibleMove(int row, int column, int number, SudokuPattern pattern, int[] possible, int[] rowsAffected, int[] columnsAffected, int[] numbersAffected) : base(row, column, number, pattern, possible)
		{
			this._ColumnsAffected = columnsAffected ?? new int[0];
			this._NumbersAffected = numbersAffected ?? new int[0];
			this._RowsAfffected = rowsAffected ?? new int[0];
		}

		public override String ToString() => this.ToString(SudokuMove.DefaultPatternNamePadding);

		public virtual String ToString(int patternNamePadding)
		{
			int[] rows = new int[this._RowsAfffected.Length];
			int[] columns = new int[this._ColumnsAffected.Length];

			for (int i = 0; i < rows.Length; i ++)
			{
				rows[i] = this._RowsAfffected[i] + 1;
			}

			for (int i = 0; i < columns.Length; i ++)
			{
				columns[i] = this._ColumnsAffected[i] + 1;
			}

			return String.Join("\t", this.ToString(patternNamePadding, true), String.Join(",", rows).PadRight(patternNamePadding), String.Join(",", columns).PadRight(patternNamePadding), String.Join(",", this._NumbersAffected).PadRight(patternNamePadding));
		}
	}
}
