using System;
using System.Diagnostics;

using AndroidX.Lifecycle;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewModel : ViewModel
	{
		private const int DefaultColumn = -1;
		private const int DefaultRow = -1;

		private ControlEventArgs _State;
		private readonly Object CompletedLockObject;
		private readonly Object StateChangedLockObject;

		private event EventHandler _Completed;
		private event ControlEventHandler _StateChanged;

		public ControlEventArgs State
		{
			get => this._State;
			set => this.Update(value);
		}

		public int Column { get; private set; }
		public bool IsCorrect { get; private set; }
		public bool IsRunning => this.Stopwatch.IsRunning;
		public int MoveCount { get; private set; }
		public int Row { get; private set; }
		private Stopwatch Stopwatch { get; }
		public SudokuPuzzle Sudoku { get; }
		public double TimeOnStop { get; private set; }

		public event EventHandler Completed
		{
			add
			{
				lock (this.CompletedLockObject)
				{
					this._Completed += value;
				}
			}

			remove
			{
				lock (this.CompletedLockObject)
				{
					this._Completed -= value;
				}
			}
		}

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
			this.CompletedLockObject = new Object();
			this.StateChangedLockObject = new Object();
			this._Completed = null;
			this._StateChanged = null;
			this.Column = SudokuViewModel.DefaultColumn;
			this.IsCorrect = false;
			this.MoveCount = 0;
			this.Row = SudokuViewModel.DefaultRow;
			this.Stopwatch = new Stopwatch();
			this.Sudoku = SudokuGenerator.AddNumbers(9, SudokuDifficulty.Easy);
			this.TimeOnStop = 0;
			SudokuGenerator.RemoveNumbers(this.Sudoku);
		}

		private void MakeMove(int row, int column, int number)
		{
			this.Sudoku[row, column] = number;
			this.MoveCount ++;
			this.Stopwatch.Start();
			double time = this.Stopwatch.ElapsedMilliseconds;

			if (this.Sudoku.IsComplete)
			{
				this.IsCorrect = this.Sudoku.Check();
				this.Stopwatch.Stop();
				this.TimeOnStop = time;

				if (!(this._Completed is null))
				{
					this._Completed(this, new EventArgs());
				}
			}
		}

		public void Pause() => this.Stopwatch.Stop();
		public void Resume() => this.Stopwatch.Start();

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
							this.MakeMove(row, column, 0);
						}

						break;

					case ControlEvent.Number:
						if (!this.Sudoku.CheckReadOnly(row, column))
						{
							this.MakeMove(row, column, this.Sudoku[row, column] == this._State.Number ? 0 : this._State.Number);
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
						this.MakeMove(this.Row, this.Column, 0);
						break;

					case ControlEvent.Number:
						this.MakeMove(this.Row, this.Column, this.Sudoku[this.Row, this.Column] == state.Number ? 0 : state.Number);
						break;
				}

				state = ControlEventArgs.Empty;
			}

			this._State = state;
			
			if (!(this._StateChanged is null))
			{
				this._StateChanged(this, this._State);
			}
		}
	}
}