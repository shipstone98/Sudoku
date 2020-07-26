using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using SystemExtensions;

[assembly: InternalsVisibleTo("Sudoku.Test")]
namespace Sudoku
{
	/// <summary>
	/// Represents a sudoku puzzle.
	/// </summary>
	public class Sudoku: ICloneable, IEquatable<Sudoku>
	{
		private SudokuCell[,] Cells { get; }

		public SudokuDifficulty Difficulty { get; }

		/// <summary>
		/// Gets the total number of elements in each row or column of the <see cref="Sudoku"/> puzzle.
		/// </summary>
		public int Size { get; }

		internal int SizeSqrt { get; }

		public int this[int row, int column]
		{
			get
			{
				this.Check(row, nameof (row));
				this.Check(column, nameof (column));
				return this.Cells[row, column].Number;
			}

			set
			{
				this.Check(row, nameof (row));
				this.Check(column, nameof (column));

				if (this.Cells[row, column].Number == value)
				{
					return;
				}

				this.Check(value, nameof (value), true);

				if (this.Cells[row, column].ReadOnly)
				{
					throw new SudokuCellReadOnlyException(row, column);
				}

				int oldValue = this.Cells[row, column].Number;
				this.Cells[row, column].Number = value;

				//If newValue 0, add the currentValue to all possible cells in row, column and block
				if (value == 0)
				{
					this.AddAllPossible(row, column, oldValue);
				}

				//Else if currentValue is 0, remove the newValue from all possible cells
				else if (this.Cells[row, column].Number == 0)
				{
					this.RemoveAllPossible(row, column, value);
				}

				//Else, add the currentValue and remove the newValue from all possible cells
				else
				{
					this.AddAllPossible(row, column, oldValue);
					this.RemoveAllPossible(row, column, value);
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="difficulty"></param>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0.</exception>
		public Sudoku(int size, SudokuDifficulty difficulty)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof (size));
			}

			if (!MathExtensions.IsSquare((ulong) size))
			{
				throw new ArgumentException(nameof (size));
			}

			this.Cells = new SudokuCell[size, size];
			this.Difficulty = difficulty;
			int[] numbers = new int[size];

			for (int i = 0; i < size; i ++)
			{
				numbers[i] = i + 1;
			}

			for (int i = 0; i < size; i ++)
			{
				for (int j = 0; j < size; j ++)
				{
					this.Cells[i, j] = new SudokuCell();

					foreach (int number in numbers)
					{
						this.Cells[i, j].AddPossible(number);
					}
				}
			}

			this.Size = size;
			this.SizeSqrt = (int) Math.Sqrt(size);
		}

		internal void AddAllPossible(int row, int column, int number) => this.AddPossible(row, column, number, true, true, true);

		private bool AddPossible(int row, int column, int number)
		{
			//Check if can go here first, if not return false
			if (this.RowContains(row, number) || this.ColumnContains(column, number) || this.BlockContains(row, column, number))
			{
				return false;
			}

			this.Cells[row, column].AddPossible(number);
			return true;
		}

		internal void AddPossible(int row, int column, int number, bool addToRow, bool addToColumn, bool addToBlock) => this.ModifyPossible(row, column, number, addToRow, addToColumn, addToBlock, false);

		internal bool BlockContains(int row, int column, int number, bool ignoreCell = false)
		{
			this.GetStartRowColumn(row, column, out int startRow, out int startColumn);

			for (int i = 0; i < this.SizeSqrt; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.SizeSqrt; j ++)
				{
					int currentColumn = startColumn + j;

					if (ignoreCell && currentRow == row && currentColumn == column)
					{
						continue;
					}

					if (this.Cells[currentRow, currentColumn].Number == number)
					{
						return true;
					}
				}
			}

			return false;
		}

		private void Check(int value, String valueName, bool excludeUpperLimit = false)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(valueName);
			}

			if (value > this.Size || !excludeUpperLimit && value == this.Size)
			{
				throw new ArgumentException(valueName);
			}
		}

		internal bool ColumnContains(int column, int number) => this.ColumnContains(0, column, number);

		internal bool ColumnContains(int row, int column, int number, bool ignoreCell = false)
		{
			for (int i = 0; i < this.Size; i ++)
			{
				if (ignoreCell && row == i)
				{
					continue;
				}

				if (this.Cells[i, column].Number == number)
				{
					return true;
				}
			}

			return false;
		}

		internal bool ContainsPossible(int row, int column, int number)
		{
			this.Check(row, nameof (row));
			this.Check(column, nameof (column));
			this.Check(number, nameof (number), true);
			return this.Cells[row, column].Possible.Contains(number);
		}

		public Object Clone()
		{
			Sudoku clone = new Sudoku(this.Size, this.Difficulty);

			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					clone.Cells[i, j] = (SudokuCell) this.Cells[i, j].Clone();
				}
			}

			return clone;
		}

		public override bool Equals(Object obj) => this.Equals(obj as Sudoku);

		public bool Equals(Sudoku sudoku)
		{
			if (sudoku is null || this.Size != sudoku.Size)
			{
				return false;
			}

			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					if (this.Cells[i, j].Number != sudoku.Cells[i, j].Number)
					{
						return false;
					}
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			const int RANDOM = 78459;
			int hash = RANDOM * this.Difficulty.GetHashCode() * this.Size.GetHashCode();

			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					hash *= this.Cells[i, j].GetHashCode();
				}
			}

			return hash;
		}

		internal int[] GetPossible(int row, int column) => this.Cells[row, column].Possible;

		internal void GetStartRowColumn(int row, int column, out int startRow, out int startColumn)
		{
			startRow = row - row % this.SizeSqrt;
			startColumn = column - column % this.SizeSqrt;
		}

		internal void ModifyPossible(int row, int column, int number, bool modifyRow, bool modifyColumn, bool modifyBlock, bool remove)
		{
			if (remove)
			{
				this.RemovePossible(row, column, number);
			}

			else
			{
				this.AddPossible(row, column, number);
			}

			//Rows
			if (modifyRow)
			{
				for (int i = 0; i < this.Size; i++)
				{
					if (i == column)
					{
						continue;
					}

					if (remove)
					{
						this.RemovePossible(row, i, number);
					}

					else
					{
						this.AddPossible(row, i, number);
					}
				}
			}

			//Columns
			if (modifyColumn)
			{
				for (int i = 0; i < this.Size; i++)
				{
					if (i == row)
					{
						continue;
					}

					if (remove)
					{
						this.RemovePossible(i, column, number);
					}

					else
					{
						this.AddPossible(i, column, number);
					}
				}
			}

			//Block
			if (modifyBlock)
			{
				this.GetStartRowColumn(row, column, out int startRow, out int startColumn);

				for (int i = 0; i < this.SizeSqrt; i++)
				{
					int currentRow = startRow + i;

					if (modifyRow && currentRow == row)
					{
						continue;
					}

					for (int j = 0; j < this.SizeSqrt; j++)
					{
						int currentColumn = startColumn + j;

						if ((currentRow == row || modifyColumn) && currentColumn == column)
						{
							continue;
						}

						if (remove)
						{
							this.RemovePossible(currentRow, currentColumn, number);
						}

						else
						{
							this.AddPossible(currentRow, currentColumn, number);
						}
					}
				}
			}
		}

		public static Sudoku Parse(String s, SudokuDifficulty difficulty = SudokuDifficulty.None)
		{
			if (s is null)
			{
				throw new ArgumentNullException(nameof (s));
			}

			s = Regex.Replace(s, @"[^0-9]", "");
			int size = (int) Math.Sqrt(s.Length);

			if (!Sudoku.VerifySize(size))
			{
				throw new FormatException();
			}

			Sudoku sudoku = new Sudoku(size, difficulty);

			for (int i = 0; i < size; i ++)
			{
				int currentRow = i * size;

				for (int j = 0; j < size; j ++)
				{
					sudoku[i, j] = s[currentRow + j] - '0';
				}
			}

			return sudoku;
		}

		internal void RemoveAllPossible(int row, int column, int number) => this.RemovePossible(row, column, number, true, true, true);
		internal bool RemovePossible(int row, int column, int number) => this.Cells[row, column].RemovePossible(number);
		internal void RemovePossible(int row, int column, int number, bool removeRow, bool removeColumn, bool removeBlock) => this.ModifyPossible(row, column, number, removeRow, removeColumn, removeBlock, true);
		internal bool RowContains(int row, int number) => this.RowContains(row, 0, number);

		internal bool RowContains(int row, int column, int number, bool ignoreCell = false)
		{
			for (int i = 0; i < this.Size; i ++)
			{
				if (ignoreCell && column == i)
				{
					continue;
				}

				if (this.Cells[row, i].Number == number)
				{
					return true;
				}
			}

			return false;
		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			int length = this.Size * 2 + this.SizeSqrt * 2 + 1;
			String line = new String('-', length);

			for (int i = 0; i < this.Size; i ++)
			{
				if (i % this.SizeSqrt == 0)
				{
					sb.AppendLine(line);
				}

				for (int j = 0; j < this.Size; j ++)
				{
					if (j % this.SizeSqrt == 0)
					{
						sb.Append("| ");
					}

					int number = this.Cells[i, j].Number;
					sb.Append(number == 0 ? "  " : number + " ");
				}

				sb.AppendLine("|");
			}

			sb.Append(line);
			return sb.ToString();
		}

		public static bool VerifySize(int size) => size > 0 && MathExtensions.IsSquare((ulong) size);

		public static bool operator ==(Sudoku a, Sudoku b) => a is null ? b is null : a.Equals(b);
		public static bool operator !=(Sudoku a, Sudoku b) => !(a == b);
	}
}
