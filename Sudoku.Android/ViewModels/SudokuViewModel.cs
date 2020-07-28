using AndroidX.Lifecycle;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewModel : ViewModel
	{
		private const int DefaultColumn = -1;
		private const int DefaultRow = -1;

		private ControlEventArgs _State;

		public int Column { get; private set; }
		public int Row { get; private set; }

		public ControlEventArgs State
		{
			get => this._State;
			set => this.Update(value ?? ControlEventArgs.Empty);
		}

		public Sudoku Sudoku { get; set; }

		public SudokuViewModel()
		{
			this._State = ControlEventArgs.Empty;
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

		private void Update(ControlEventArgs state)
		{
			switch (state.Event)
			{
				case ControlEvent.Clear:
					if (this.Row < 0 || this.Column < 0 || this.Sudoku.CheckReadOnly(this.Row, this.Column))
					{
						break;
					}

					this.Sudoku[this.Row, this.Column] = 0;
					state = ControlEventArgs.Empty;
					break;

				case ControlEvent.Number:
					if (this.Row < 0 || this.Column < 0 || this.Sudoku.CheckReadOnly(this.Row, this.Column))
					{
						break;
					}

					this.Sudoku[this.Row, this.Column] = state.Number;
					state = ControlEventArgs.Empty;
					break;
			}

			this._State = state;
		}
	}
}