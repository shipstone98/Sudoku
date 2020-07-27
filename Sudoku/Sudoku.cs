using System;
using System.Collections;
using System.Collections.Generic;
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
	public class Sudoku: ICloneable, IEnumerable, IEnumerable<IEnumerable<int>>, IEquatable<Sudoku>
	{
		/// <summary>
		/// Represents the largest possible number of rows and columns in a <see cref="Sudoku"/> puzzle. This field is constant.
		/// </summary>
		public const int MaximumSupportedSize = 9;

		private readonly Object LockObject;

		private event SudokuChangedEventHandler _Changed;

		/// <summary>
		/// Gets the number of elements in each row or column of a block, or sub-grid, contained in the <see cref="Sudoku"/> puzzle. This is the positive square root of the <see cref="Size"/> property.
		/// </summary>
		public int BlockSize { get; }

		private SudokuCell[,] Cells { get; }

		/// <summary>
		/// Gets an enumeration containing the columns in the <see cref="Sudoku"/> puzzle.
		/// </summary>
		public IEnumerable<IEnumerable<int>> Columns
		{
			get
			{
				int[][] columns = new int[this.Size][];

				for (int i = 0; i < this.Size; i ++)
				{
					columns[i] = new int[this.Size];

					for (int j = 0; j < this.Size; j ++)
					{
						columns[i][j] = this.Cells[j, i].Number;
					}
				}

				return columns;
			}
		}

		/// <summary>
		/// Gets the difficulty associated with the current <see cref="Sudoku"/> puzzle.
		/// </summary>
		public SudokuDifficulty Difficulty { get; }

		internal bool DisablePossible { get; set; }

		/// <summary>
		/// Gets a value that indicates whether the puzzle is complete (i.e. there are no empty cells). This does not mean the puzzle is completed correctly.
		/// </summary>
		public bool IsComplete
		{
			get
			{
				foreach (IEnumerable<int> row in this)
				{
					foreach (int cell in row)
					{
						if (cell == 0)
						{
							return false;
						}
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Gets an enumeration containing the rows in the <see cref="Sudoku"/> puzzle.
		/// </summary>
		public IEnumerable<IEnumerable<int>> Rows
		{
			get
			{
				int[][] rows = new int[this.Size][];

				for (int i = 0; i < this.Size; i ++)
				{
					rows[i] = new int[this.Size];

					for (int j = 0; j < this.Size; j ++)
					{
						rows[i][j] = this.Cells[i, j].Number;
					}
				}

				return rows;
			}
		}

		/// <summary>
		/// Gets the total number of elements in each row or column of the <see cref="Sudoku"/> puzzle.
		/// </summary>
		public int Size { get; }

		/// <summary>
		/// Occurs when the number in a cell in the <see cref="Sudoku"/> puzzle is changed.
		/// </summary>
		public event SudokuChangedEventHandler Changed
		{
			add
			{
				lock (this.LockObject)
				{
					this._Changed += value;
				}
			}

			remove
			{
				lock (this.LockObject)
				{
					this._Changed -= value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the number at the specified row and column.
		/// </summary>
		/// <param name="row">The zero-based row index of the number to get or set.</param>
		/// <param name="column">The zero-based column index of the number to get or set.</param>
		/// <returns>The number at the specified row and column.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="row"/></c> is greater than or equal to the <see cref="Size"/> property - or - <c><paramref name="column"/></c> is greater than or equal to the <see cref="Size"/> property - or - <c>value</c> is greater than the <see cref="Size"/> property.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c> is less than 0 - or - <c><paramref name="column"/></c> is less than 0 - or - <c>value</c> is less than 0.</exception>
		/// <exception cref="SudokuCellReadOnlyException">The cell specified at <c><paramref name="row"/></c>, <c><paramref name="column"/></c> is read-only.</exception>
		public int this[int row, int column]
		{
			get
			{
				this.CheckArgument(row, nameof (row));
				this.CheckArgument(column, nameof (column));
				return this.Cells[row, column].Number;
			}

			set
			{
				this.CheckArgument(row, nameof (row));
				this.CheckArgument(column, nameof (column));

				if (this.Cells[row, column].Number == value)
				{
					return;
				}

				this.CheckArgument(value, nameof (value), true);

				if (this.Cells[row, column].IsReadOnly)
				{
					throw new SudokuCellReadOnlyException(row, column);
				}

				int oldValue = this.Cells[row, column].Number;
				this.Cells[row, column].Number = value;
				
				if (!(this._Changed is null))
				{
					this._Changed(this, new SudokuEventArgs(row, column, value));
				}

				if (!this.DisablePossible)
				{
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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sudoku"/> class that is empty and has the specified <c><paramref name="size"/></c> and <c><paramref name="difficulty"/></c>.
		/// </summary>
		/// <param name="size">The number of elements that the new sudoku can store in each row, column and block.</param>
		/// <param name="difficulty">The difficulty associated with the <see cref="Sudoku"/> puzzle.</param>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0 or greater than <see cref="Sudoku.MaximumSupportedSize"/>.</exception>
		public Sudoku(int size, SudokuDifficulty difficulty)
		{
			if (size <= 0 || size > Sudoku.MaximumSupportedSize)
			{
				throw new ArgumentOutOfRangeException(nameof (size));
			}

			if (!MathExtensions.IsSquare((ulong) size))
			{
				throw new ArgumentException(nameof (size));
			}

			this.LockObject = new Object();
			this._Changed = null;
			this.Cells = new SudokuCell[size, size];
			this.Difficulty = difficulty;
			this.DisablePossible = false;
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
			this.BlockSize = (int) Math.Sqrt(size);
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

			for (int i = 0; i < this.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.BlockSize; j ++)
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

		/// <summary>
		/// Determines whether the numbers in all cells of the <see cref="Sudoku"/> puzzle match the same cell's solution.
		/// </summary>
		/// <returns><c>true</c> the numbers in all cells of the <see cref="Sudoku"/> puzzle match the same cell's solution; otherwise, false.</returns>
		public bool Check()
		{
			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					if (!this.Cells[i, j].IsCorrect)
					{
						return false;
					}
				}
			}

			return true;
		}

		private void CheckArgument(int value, String valueName, bool excludeUpperLimit = false)
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

		/// <summary>
		/// Determines whether a specified cell in the <see cref="Sudoku"/> puzzle is read-only.
		/// </summary>
		/// <param name="row">The zero-based row of the cell.</param>
		/// <param name="column">The zero-based column of the cell.</param>
		/// <returns><c>true</c> if the specified cell at <c><paramref name="row"/></c> and <c><paramref name="column"/></c> is read-only; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c> is less than 0 - or - <c><paramref name="column"/></c> is less than 0.</exception>
		public bool CheckReadOnly(int row, int column)
		{
			this.CheckArgument(row, nameof (row));
			this.CheckArgument(column, nameof (column));
			return this.Cells[row, column].IsReadOnly;
		}

		/// <summary>
		/// Creates a new <see cref="Sudoku"/> puzzle that is a copy of the current instance.
		/// </summary>
		/// <returns>A new <see cref="Sudoku"/> puzzle that is a copy of this instance.</returns>
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

		/// <summary>
		/// Determines whether a number is in the collection of possible values at a specified <c><paramref name="row"/></c> and <c><paramref name="column"/></c> in the <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <param name="row">The zero-based row index.</param>
		/// <param name="column">The zero-based column index.</param>
		/// <param name="number">The number to locate in the collection of possible values.</param>
		/// <returns><c>true</c> if <c><paramref name="number"/></c> is found in the collection of possible values; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="row"/></c> is greater than or equal to the <see cref="Size"/> property - or - <c><paramref name="column"/></c> is greater than or equal to the <see cref="Size"/> property - or - <c><paramref name="number"/></c> is greater than the <see cref="Size"/> property.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c> is less than 0 - or - <c><paramref name="column"/></c> is less than 0 - or - <c><paramref name="number"/></c> is less than 0.</exception>
		public bool ContainsPossible(int row, int column, int number)
		{
			this.CheckArgument(row, nameof (row));
			this.CheckArgument(column, nameof (column));
			this.CheckArgument(number, nameof (number), true);
			return this.Cells[row, column].ContainsPossible(number);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <param name="obj">The object to compare with the current <see cref="Sudoku"/> puzzle.</param>
		/// <returns><c>true</c> if the specified object is equal to the current <see cref="Sudoku"/> puzzle; otherwise, <c>false</c>.</returns>
		public override bool Equals(Object obj) => this.Equals(obj as Sudoku);

		/// <summary>
		/// Determines whether the specified <c><paramref name="sudoku"/></c> is equal to the current <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <param name="sudoku">The puzzle to compare with the current <see cref="Sudoku"/> puzzle.</param>
		/// <returns><c>true</c> if <c><paramref name="sudoku"/></c> is equal to the current <see cref="Sudoku"/> puzzle; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Returns an enumerator that iterates through the rows of the <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the rows of the <see cref="Sudoku"/> puzzle.</returns>
		public IEnumerator<IEnumerable<int>> GetEnumerator() => this.Rows.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			const int RANDOM = 78459;
			int hash = RANDOM * this.Size.GetHashCode();

			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					hash *= this.Cells[i, j].GetHashCode();
				}
			}

			return hash;
		}

		/// <summary>
		/// Creates a new enumeration containing the zero-based row and column indices of all empty and incorrect cells in the <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <returns>A new enumeration containing the zero-based row and column indices of all empty and incorrect cells in the <see cref="Sudoku"/> puzzle.</returns>
		public IEnumerable<Tuple<int, int>> GetIncorrect()
		{
			List<Tuple<int, int>> cells = new List<Tuple<int, int>>();
			
			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					if (!this.Cells[i, j].IsCorrect)
					{
						cells.Add(new Tuple<int, int>(i, j));
					}
				}
			}

			return cells;
		}

		/// <summary>
		/// Copies the elements of the collection of possible values at the specified <c><paramref name="row"/></c> and <c><paramref name="column"/></c> in the <see cref="Sudoku"/> puzzle to a new array.
		/// </summary>
		/// <param name="row">The zero-based row index.</param>
		/// <param name="column">The zero-based column index.</param>
		/// <returns>An array containing copies of the possible values.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="row"/></c> is greater than or equal to the <see cref="Size"/> property - or - <c><paramref name="column"/></c> is greater than or equal to the <see cref="Size"/> property.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c> is less than 0 - or - <c><paramref name="column"/></c> is less than 0.</exception>
		public int[] GetPossible(int row, int column)
		{
			this.CheckArgument(row, nameof (row));
			this.CheckArgument(column, nameof (column));
			return this.Cells[row, column].Number == 0 ? this.Cells[row, column].Possible : null;
		}

		internal void GetStartRowColumn(int row, int column, out int startRow, out int startColumn)
		{
			startRow = row - row % this.BlockSize;
			startColumn = column - column % this.BlockSize;
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

				for (int i = 0; i < this.BlockSize; i++)
				{
					int currentRow = startRow + i;

					if (modifyRow && currentRow == row)
					{
						continue;
					}

					for (int j = 0; j < this.BlockSize; j++)
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

		/// <summary>
		/// Converts the string representation to its <see cref="Sudoku"/> puzzle equivalent.
		/// </summary>
		/// <param name="s">A string containing a puzzle to convert.</param>
		/// <param name="difficulty">An optional difficulty to associate with the <see cref="Sudoku"/> puzzle.</param>
		/// <returns>A <see cref="Sudoku"/> puzzle equivalent to the puzzle contained in <c><paramref name="s"/></c>.</returns>
		/// <exception cref="ArgumentNullException"><c><paramref name="s"/></c> is <c>null</c>.</exception>
		/// <exception cref="FormatException"><c><paramref name="s"/></c> is not in the correct format.</exception>
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

			Sudoku sudoku;

			try
			{
				sudoku = new Sudoku(size, difficulty);
			}

			catch
			{
				throw new FormatException();
			}

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
		
		internal void ResetPossible()
		{
			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					this.Cells[i, j].ResetPossible();

					for (int k = 1; k <= this.Size; k ++)
					{
						if (!(this.RowContains(i, k) && this.ColumnContains(j, k) && this.BlockContains(i, j, k)))
						{
							this.Cells[i, j].AddPossible(k);
						}
					}
				}
			}
		}
		
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

		internal void SetReadOnlyProperties()
		{
			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					if (this.Cells[i, j].Number != 0)
					{
						this.Cells[i, j].IsReadOnly = true;
					}
				}
			}
		}

		internal void SetSolutions()
		{
			for (int i = 0; i < this.Size; i ++)
			{
				for (int j = 0; j < this.Size; j ++)
				{
					this.Cells[i, j].Solution = this.Cells[i, j].Number;
				}
			}
		}

		/// <summary>
		/// Returns a string that represents the current <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <returns>A string that represents the current <see cref="Sudoku"/> puzzle.</returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			int length = this.Size * 2 + this.BlockSize * 2 + 1;
			String line = new String('-', length);

			for (int i = 0; i < this.Size; i ++)
			{
				if (i % this.BlockSize == 0)
				{
					sb.AppendLine(line);
				}

				for (int j = 0; j < this.Size; j ++)
				{
					if (j % this.BlockSize == 0)
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

		/// <summary>
		/// Determines whether the specified <c><paramref name="size"/></c> is acceptable for a <see cref="Sudoku"/> puzzle.
		/// </summary>
		/// <param name="size">The size to verify.</param>
		/// <returns><c>true</c> if the specified <c><paramref name="size"/></c> is acceptable for a <see cref="Sudoku"/> puzzle; otherwise, <c>false</c>.</returns>
		public static bool VerifySize(int size) => size > 0 && size <= Sudoku.MaximumSupportedSize && MathExtensions.IsSquare((ulong) size);

		/// <summary>
		/// Determines whether two specified <see cref="Sudoku"/> puzzles have the same numbers in all cells.
		/// </summary>
		/// <param name="a">The first <see cref="Sudoku"/> puzzle to compare, or <c>null</c>.</param>
		/// <param name="b">The second <see cref="Sudoku"/> puzzle to compare, or <c>null</c>.</param>
		/// <returns><c>true</c> if the number in each cell in <c><paramref name="a"/></c> is the same as the number in the same cell in <c><paramref name="b"/></c>; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Sudoku a, Sudoku b) => a is null ? b is null : a.Equals(b);

		/// <summary>
		/// Determines whether two specified <see cref="Sudoku"/> puzzles have the same numbers in any cell.
		/// </summary>
		/// <param name="a">The first <see cref="Sudoku"/> puzzle to compare, or <c>null</c>.</param>
		/// <param name="b">The second <see cref="Sudoku"/> puzzle to compare, or <c>null</c>.</param>
		/// <returns><c>true</c> if the number in any cell in <c><paramref name="a"/></c> is different to the number in the same cell in <c><paramref name="b"/></c>; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Sudoku a, Sudoku b) => !(a == b);
	}
}
