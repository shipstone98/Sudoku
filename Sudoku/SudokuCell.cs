using System;

namespace Sudoku
{
	internal class SudokuCell
	{
		private int _Number;
		private int _Solution;

		internal bool Correct => this._Number == this._Solution && this._Number != 0;

		internal int Number
		{
			get => this._Number;

			set
			{
				if (this.ReadOnly)
				{
					throw new SudokuCellReadOnlyException();
				}

				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof (value));
				}

				this._Number = value;
			}
		}

		internal bool ReadOnly { get; private set; }

		internal int Solution
		{
			get => this._Solution;

			set
			{
				if (this.ReadOnly)
				{
					throw new SudokuCellReadOnlyException();
				}

				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this._Solution = value;
			}
		}

		internal SudokuCell()
		{
			this._Number = this._Solution = 0;
			this.ReadOnly = false;
		}

		internal void MakeReadOnly() => this.ReadOnly = true;
	}
}
