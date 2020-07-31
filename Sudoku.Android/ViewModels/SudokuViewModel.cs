using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using AndroidX.Lifecycle;

using Java.IO;

namespace Sudoku.Android.ViewModels
{
	public class SudokuViewModel : ViewModel
	{
		private const int DefaultColumn = -1;
		private const int DefaultRow = -1;

		private ControlEventArgs _State;
		private readonly Object CompletedLockObject;
		private readonly Object StateChangedLockObject;
		private readonly Object SudokuChangedLockObject;

		private event EventHandler _Completed;
		private event ControlEventHandler _StateChanged;
		private event EventHandler _SudokuChanged;

		public ControlEventArgs State
		{
			get => this._State;
			set => this.Update(value);
		}

		public ActionType Action { get; set; }
		public int Column { get; private set; }
		public bool IsCorrect { get; private set; }
		public bool IsRunning => this.Stopwatch.IsRunning;
		public int MoveCount { get; private set; }
		public int Row { get; private set; }
		private Stopwatch Stopwatch { get; }
		public SudokuPuzzle Sudoku { get; private set; }
		private double TimeOnStart { get; set; }
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

		public event EventHandler SudokuChanged
		{
			add
			{
				lock (this.SudokuChangedLockObject)
				{
					this._SudokuChanged += value;
				}
			}

			remove
			{
				lock (this.SudokuChangedLockObject)
				{
					this._SudokuChanged -= value;
				}
			}
		}

		public SudokuViewModel()
		{
			this._State = ControlEventArgs.Empty;
			this.CompletedLockObject = new Object();
			this.StateChangedLockObject = new Object();
			this.SudokuChangedLockObject = new Object();
			this._Completed = null;
			this._StateChanged = null;
			this._SudokuChanged = null;
			this.Action = ActionType.None;
			this.Column = SudokuViewModel.DefaultColumn;
			this.IsCorrect = false;
			this.MoveCount = 0;
			this.Row = SudokuViewModel.DefaultRow;
			this.Stopwatch = new Stopwatch();
			this.Sudoku = null;
			this.TimeOnStart = this.TimeOnStop = 0;
		}

		private void MakeMove(int row, int column, int number)
		{
			if (this.Sudoku is null)
			{
				return;
			}

			this.Sudoku[row, column] = number;
			this.MoveCount ++;
			this.Stopwatch.Start();
			double time = this.Stopwatch.ElapsedMilliseconds;

			if (this.Sudoku.IsComplete)
			{
				this.IsCorrect = this.Sudoku.Check();
				this.Stopwatch.Stop();
				this.TimeOnStop = time + this.TimeOnStart;

				if (!(this._Completed is null))
				{
					this._Completed(this, EventArgs.Empty);
				}

				//Clear move and timer count file
				//Deleted saved file
				//Check high score for that difficulty
			}
		}

		public async Task<bool> OpenAsync(File dirname, SudokuDifficulty difficulty)
		{
			if (!(this.Sudoku is null))
			{
				return false;
			}

			if (difficulty == SudokuDifficulty.None)
			{
				File file;

				try
				{
					file = new File(dirname, "Sudoku.txt");
				}

				catch
				{
					return false;
				}

				if (!file.Exists())
				{
					return false;
				}

				FileInputStream fis;

				try
				{
					fis = new FileInputStream(file);
				}

				catch
				{
					return false;
				}

				const int BUFFER_SIZE = 1024;
				byte[] array = new byte[BUFFER_SIZE];
				int n;
				StringBuilder sb = new StringBuilder();

				do
				{
					n = await fis.ReadAsync(array, 0, BUFFER_SIZE);

					if (n < BUFFER_SIZE)
					{
						String s = Encoding.Default.GetString(array).Substring(0, n);
						sb.Append(s);
						break;
					}

					sb.Append(Encoding.Default.GetString(array));
				} while (true);

				try
				{
					String s = sb.ToString();
					this.Sudoku = SudokuSerializer.Deserialize(s);
					this._SudokuChanged(this, EventArgs.Empty);
				}

				catch (FormatException)
				{
					return false;
				}
			}

			else if (this.Action == ActionType.Generate)
			{
				this.Sudoku = new SudokuPuzzle(9, SudokuDifficulty.None);
				this._SudokuChanged(this, EventArgs.Empty);
			}

			else
			{
				this.Sudoku = SudokuGenerator.Generate(9, difficulty);
				this._SudokuChanged(this, EventArgs.Empty);
			}

			return true;
		}

		public void Pause() => this.Stopwatch.Stop();

		public void Resume()
		{
			if (this.Stopwatch.ElapsedMilliseconds > 0 || this.TimeOnStart > 0)
			{
				this.Stopwatch.Start();
			}
		}

		public async Task<bool> SaveAsync(File dirname)
		{
			if (this.Sudoku is null)
			{
				return false;
			}

			File file;

			try
			{
				file = new File(dirname, "Sudoku.txt");
			}

			catch
			{
				return false;
			}

			if (!file.Exists())
			{
				file.CreateNewFile();
			}

			FileOutputStream fos;

			try
			{
				fos = new FileOutputStream(file, false);
			}

			catch
			{
				return false;
			}

			String s = SudokuSerializer.Seralize(this.Sudoku);
			byte[] array = Encoding.Default.GetBytes(s);
			await fos.WriteAsync(array);
			await fos.FlushAsync();
			fos.Close();
			return true;
		}

		public void SetRowAndColumn(int row, int column)
		{
			if (this.Sudoku is null)
			{
				this.Column = SudokuViewModel.DefaultColumn;
				this.Row = SudokuViewModel.DefaultRow;
				this.State = ControlEventArgs.Empty;
				return;
			}

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

		public async Task<bool> StartAsync(File dirname)
		{
			if (this.TimeOnStop == 0 && this.MoveCount == 0)
			{
				if (!(this.Sudoku is null))
				{
					return false;
				}

				File file;

				try
				{
					file = new File(dirname, "Save.txt");
				}

				catch
				{
					return false;
				}

				if (!file.Exists())
				{
					file.CreateNewFile();
				}

				FileInputStream fis;

				try
				{
					fis = new FileInputStream(file);
				}

				catch
				{
					return false;
				}

				const int BUFFER_SIZE = 1024;
				byte[] array = new byte[BUFFER_SIZE];
				int n;
				StringBuilder sb = new StringBuilder();

				do
				{
					n = await fis.ReadAsync(array, 0, BUFFER_SIZE);

					if (n < BUFFER_SIZE)
					{
						String s = Encoding.Default.GetString(array).Substring(0, n);
						sb.Append(s);
						break;
					}

					sb.Append(Encoding.Default.GetString(array));
				} while (true);

				String[] lines = sb.ToString().Replace("\r", String.Empty).ToLower().Split('\n');

				foreach (String line in lines)
				{
					int index = line.IndexOf(':') + 1;

					if (line.StartsWith("moves"))
					{
						this.MoveCount = Int32.Parse(line.Substring(index));
					}

					else if (line.StartsWith("time"))
					{
						this.TimeOnStart = Double.Parse(line.Substring(index));
					}
				}

				fis.Close();
			}

			return true;
		}

		public async Task<bool> StopAsync(File dirname)
		{
			if (this.Sudoku is null)
			{
				return false;
			}

			File file;

			try
			{
				file = new File(dirname, "Save.txt");
			}

			catch
			{
				return false;
			}

			if (!file.Exists())
			{
				file.CreateNewFile();
			}

			FileOutputStream fos;

			try
			{
				fos = new FileOutputStream(file, false);
			}

			catch
			{
				return false;
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("time:" + this.Stopwatch.ElapsedMilliseconds);
			sb.AppendLine("moves:" + this.MoveCount);
			byte[] array = Encoding.Default.GetBytes(sb.ToString());
			await fos.WriteAsync(array);
			await fos.FlushAsync();
			fos.Close();
			return true;
		}

		private void Update(ControlEventArgs state)
		{
			if (this.Sudoku is null)
			{
				this.Column = SudokuViewModel.DefaultColumn;
				this.Row = SudokuViewModel.DefaultRow;
				this.State = ControlEventArgs.Empty;
				return;
			}

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