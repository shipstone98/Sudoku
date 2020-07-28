using System;

using AndroidX.Lifecycle;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewModel : ViewModel
	{
		private const int DefaultColumn = -1;
		private const int DefaultRow = -1;

		private ControlEventArgs _State;
		private readonly Object StateChangedLockObject;

		private event ControlEventHandler _StateChanged;

		public ControlEventArgs State
		{
			get => this._State;
			set => this.Update(value);
		}

		public int Column { get; private set; }
		public int Row { get; private set; }
		public Sudoku Sudoku { get; }

		public event ControlEventHandler StateChanged
		{
			add
			{
				lock (this.StateChangedLockObject)
				{
					this._StateChanged += value;
				}
			}

			remove
			{
				lock (this.StateChangedLockObject)
				{
					this._StateChanged -= value;
				}
			}
		}

		public SudokuViewModel()
		{
			this._State = ControlEventArgs.Empty;
			this.StateChangedLockObject = new Object();
			this._StateChanged = null;
			this.Column = SudokuViewModel.DefaultColumn;
			this.Row = SudokuViewModel.DefaultRow;
			this.Sudoku = SudokuGenerator.AddNumbers(9, SudokuDifficulty.Easy);
			SudokuGenerator.RemoveNumbers(this.Sudoku);
		}

		public void SetRowAndColumn(int row, int column)
		{
			if (row < 0 || column < 0 || this.Sudoku.CheckReadOnly(row, column))
			{
				this.Column = SudokuViewModel.DefaultColumn;
				this.Row = SudokuViewModel.DefaultRow;
			}

			if (this._State == ControlEventArgs.Empty)
			{
				if (this.Row == row && this.Column == column || this.Sudoku.CheckReadOnly(row, column))
				{
					column = SudokuViewModel.DefaultColumn;
					row = SudokuViewModel.DefaultRow;
				}

				this.Row = row;
				this.Column = column;
			}

			else
			{
				switch (this._State.Event)
				{
					case ControlEvent.Clear:
						if (!this.Sudoku.CheckReadOnly(row, column))
						{
							this.Sudoku[row, column] = 0;
						}

						break;

					case ControlEvent.Number:
						if (!this.Sudoku.CheckReadOnly(row, column))
						{
							this.Sudoku[row, column] = this.Sudoku[row, column] == this._State.Number ? 0 : this._State.Number;
						}

						break;
				}

				this.Column = SudokuViewModel.DefaultColumn;
				this.Row = SudokuViewModel.DefaultRow;
			}

			if (!(this._StateChanged is null))
			{
				this._StateChanged(this, this._State);
			}
		}

		private void Update(ControlEventArgs state)
		{
			if (state == this._State)
			{
				state = ControlEventArgs.Empty;
			}

			else if (this.Row < 0 || this.Column < 0 || this.Sudoku.CheckReadOnly(this.Row, this.Column))
			{
				//Do nothing except set state
			}

			else
			{
				switch (state.Event)
				{
					case ControlEvent.Clear:
						this.Sudoku[this.Row, this.Column] = 0;
						break;

					case ControlEvent.Number:
						this.Sudoku[this.Row, this.Column] = this.Sudoku[this.Row, this.Column] == state.Number ? 0 : state.Number;
						break;
				}

				state = ControlEventArgs.Empty;
				this.Column = SudokuViewModel.DefaultColumn;
				this.Row = SudokuViewModel.DefaultRow;
			}

			this._State = state;
			
			if (!(this._StateChanged is null))
			{
				this._StateChanged(this, this._State);
			}
		}
	}
}