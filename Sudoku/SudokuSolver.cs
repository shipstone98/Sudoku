using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SystemExtensions;

namespace Sudoku
{
	/// <summary>
	/// Solves a <see cref="T:Sudoku.Sudoku"/> puzzle using strategy and pattern matching.
	/// </summary>
	public class SudokuSolver : IEnumerable<SudokuMove>
	{
		private List<SudokuMove> Moves { get; }
		private Random Random { get; }

		/// <summary>
		/// Gets the underlying <see cref="T:Sudoku.Sudoku"/> puzzle.
		/// </summary>
		public SudokuPuzzle Sudoku { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SudokuSolver"/> class with the specified <see cref="T:Sudoku.Sudoku"/> puzzle.
		/// </summary>
		/// <param name="sudoku">The <see cref="T:Sudoku.Sudoku"/> puzzle to solve.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public SudokuSolver(SudokuPuzzle sudoku)
		{
			this.Moves = new List<SudokuMove>();
			this.Random = new Random();
			this.Sudoku = sudoku ?? throw new ArgumentNullException(nameof (sudoku));
		}

		/// <summary>
		/// Determines whether a specified <c><paramref name="sudoku"/></c> can be solved.
		/// </summary>
		/// <param name="sudoku">The <see cref="T:Sudoku.Sudoku"/> puzzle to check.</param>
		/// <param name="multipleSolutions"><c>true</c> if <c><paramref name="sudoku"/></c> has multiple possible solutions; otherwise, <c>false</c>.</param>
		/// <returns><c>true</c> if <c><paramref name="sudoku"/></c> has one or more possible solution; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static bool CheckSolvable(SudokuPuzzle sudoku, out bool multipleSolutions)
		{
			if (sudoku is null)
			{
				throw new ArgumentNullException(nameof (sudoku));
			}

			int count = 0;
			SudokuSolver.RecursiveSolve((SudokuPuzzle) sudoku.Clone(), 0, 0, ref count, true);
			multipleSolutions = count > 1;
			return count != 0;
		}

		private static bool Compare<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a is null && b is null)
			{
				return true;
			}

			if (a is null && !(b is null) || !(a is null) && b is null)
			{
				return false;
			}

			int aCount = a.Count(), bCount = b.Count();

			if (aCount != bCount)
			{
				return false;
			}

			foreach (T item in a)
			{
				if (!b.Contains(item))
				{
					return false;
				}
			}

			foreach (T item in b)
			{
				if (!a.Contains(item))
				{
					return false;
				}
			}

			return true;
		}

		private bool CheckMoveMade(int row, int column, int number, SudokuPattern pattern)
		{
			if (this.Moves.Count == 0)
			{
				return false;
			}

			int count = 0;

			foreach (SudokuMove move in this)
			{
				count++;
				int nextCount = 0;

				foreach (SudokuMove nextMove in this)
				{
					nextCount++;
					if (Object.ReferenceEquals(move, nextMove))
					{
						continue;
					}

					if (SudokuSolver.Compare(move.Rows, nextMove.Rows) && SudokuSolver.Compare(move.Columns, nextMove.Columns) && SudokuSolver.Compare(move.Numbers, nextMove.Numbers) && move.Pattern == nextMove.Pattern)
					{
						return true;
					}
				}
			}

			foreach (SudokuMove move in this)
			{
				if (move.Pattern == pattern && move.Rows.Contains(row) && move.Columns.Contains(column) && move.Numbers.Contains(number))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the moves made in the <see cref="SudokuSolver"/>.
		/// </summary>
		/// <returns>An enumerator that iterates through the moves made in the <see cref="SudokuSolver"/>.</returns>
		public IEnumerator<SudokuMove> GetEnumerator() => this.Moves.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		/// Solve the specified <see cref="T:Sudoku.Sudoku"/> puzzle using recursion, otherwise known as "brute-force".
		/// </summary>
		/// <param name="sudoku">The <see cref="T:Sudoku.Sudoku"/> puzzle to solve.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static void RecursiveSolve(SudokuPuzzle sudoku)
		{
			if (sudoku is null)
			{
				throw new ArgumentNullException(nameof (sudoku));
			}

			int count = 0;
			sudoku.DisablePossible = true;
			SudokuSolver.RecursiveSolve(sudoku, 0, 0, ref count);
			sudoku.DisablePossible = false;
			sudoku.ResetPossible();
		}

		private static bool RecursiveSolve(SudokuPuzzle sudoku, int row, int column, ref int count, bool findMultiple = false)
		{
			if (sudoku[row, column] != 0)
			{
				return SudokuSolver.RecursiveSolveNextNumber(sudoku, row, column, ref count, findMultiple);
			}

			List<int> possible = new List<int>();

			for (int i = 1; i <= sudoku.Size; i ++)
			{
				if (!(sudoku.RowContains(row, i) || sudoku.ColumnContains(column, i) || sudoku.BlockContains(row, column, i)))
				{
					possible.Add(i);
				}
			}

			SudokuGenerator.ShuffleNumbers(possible);

			while (possible.Count > 0)
			{
				sudoku[row, column] = possible[0];
				bool solvable = SudokuSolver.RecursiveSolveNextNumber(sudoku, row, column, ref count, findMultiple);

				if (solvable)
				{
					if (!findMultiple || count > 1)
					{
						return true;
					}
				}

				possible.RemoveAt(0);
			}

			sudoku[row, column] = 0;
			return false;
		}

		private static bool RecursiveSolveNextNumber(SudokuPuzzle sudoku, int row, int column, ref int count, bool findMultiple = false)
		{
			int nextColumn = column + 1, nextRow = row;

			if (nextColumn == sudoku.Size)
			{
				nextRow ++;
				nextColumn = 0;
			}

			if (nextRow == sudoku.Size)
			{
				count ++;
				return true;
			}

			return SudokuSolver.RecursiveSolve(sudoku, nextRow, nextColumn, ref count, findMultiple);
		}

		/// <summary>
		/// Solve the underlying <see cref="T:Sudoku.Sudoku"/> puzzle.
		/// </summary>
		/// <returns>An enumeration of moves used to solve the <see cref="T:Sudoku.Sudoku"/> puzzle.</returns>
		public IEnumerable<SudokuMove> Solve()
		{
			while (this.SolvePass())
			{
				continue;
			}

			return this.Moves;
		}

		private bool SolvePass()
		{
			//List<Tuple<int, int>> remainingCells = new List<Tuple<int, int>>(this.EmptyCells);
			List<Tuple<int, int>> remainingCells = new List<Tuple<int, int>>();

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					if (this.Sudoku[i, j] == 0)
					{
						remainingCells.Add(new Tuple<int, int>(i, j));
					}
				}
			}

			int count = 0;

			while (remainingCells.Count > 0)
			{
				int index = this.Random.Next(remainingCells.Count);
				Tuple<int, int> selected = remainingCells[index];
				
				if (this.SolvePass(selected.Item1, selected.Item2))
				{
					count ++;
				}

				remainingCells.RemoveAt(index);
			}

			return count > 0;
		}

		private bool SolvePass(int row, int column)
		{
			int[] possible = this.Sudoku.GetPossible(row, column);
			return this.SolveNakedSingle(row, column, possible) || this.SolveHiddenSingle(row, column, possible) || this.PointingCandidate(row, column, possible) || this.ClaimingCandidate(row, column, possible);
		}

		#region Patterns
		private bool SolveHiddenSingle(int row, int column, int[] possible)
		{			
			foreach (int number in possible)
			{
				if (this.SolveHiddenSingleRow(row, column, number) || this.SolveHiddenSingleColumn(row, column, number) || this.SolveHiddenSingleBlock(row, column, number))
				{
					this.Sudoku[row, column] = number;
					this.Moves.Add(new SudokuMove(row, column, number, SudokuPattern.HiddenSingle, possible));
					return true;
				}
			}

			return false;
		}

		private bool SolveHiddenSingleBlock(int row, int column, int number)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (row == currentRow && column == currentColumn || this.Sudoku[currentRow, currentColumn] != 0)
					{
						continue;
					}

					if (this.Sudoku.ContainsPossible(currentRow, currentColumn, number))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool SolveHiddenSingleColumn(int row, int column, int number)
		{
			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (i == row || this.Sudoku[i, column] != 0)
				{
					continue;
				}

				if (this.Sudoku.ContainsPossible(i, column, number))
				{
					return false;
				}
			}

			return true;
		}

		private bool SolveHiddenSingleRow(int row, int column, int number)
		{
			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (i == column || this.Sudoku[row, i] != 0)
				{
					continue;
				}

				if (this.Sudoku.ContainsPossible(row, i, number))
				{
					return false;
				}
			}

			return true;
		}

		private bool SolveNakedSingle(int row, int column, int[] possible)
		{
			if (possible.Length == 1 && this.Sudoku[row, column] == 0)
			{
				this.Sudoku[row, column] = possible[0];
				this.Moves.Add(new SudokuMove(row, column, possible[0], SudokuPattern.NakedSingle, possible));
				return true;
			}

			return false;
		}

		private bool PointingCandidate(int row, int column, int[] possible)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);

			foreach (int number in possible)
			{
				if (this.CheckMoveMade(row, column, number, SudokuPattern.PointingCandidate))
				{
					continue;
				}

				this.PointingCandidateConfinement(row, column, startRow, startColumn, number, out bool confinedToRow, out bool confinedToColumn);

				if (confinedToRow && !confinedToColumn)
				{
					//Delete all numbers in that row outside the block
					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startColumn)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						this.Sudoku.RemovePossible(row, i, number);
					}

					List<int> thisColumns = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentColumn = startColumn + i;

						if (this.Sudoku.ContainsPossible(row, currentColumn, number))
						{
							thisColumns.Add(currentColumn);
						}
					}

					//Add all non empty cells with that number in that row in that block to the move made
					this.Moves.Add(new SudokuMove(new int[] { row }, thisColumns.ToArray(), new int[] { number }, SudokuPattern.PointingCandidate, possible));
					return true;
				}

				if (confinedToColumn && !confinedToRow)
				{
					//Delete all numbers in that row outside the block
					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startRow)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						this.Sudoku.RemovePossible(i, column, number);
					}

					List<int> thisRows = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentRow = startRow + i;

						if (this.Sudoku.ContainsPossible(currentRow, column, number))
						{
							thisRows.Add(currentRow);
						}
					}

					//Add all non empty cells with that number in that row in that block to the move made
					this.Moves.Add(new SudokuMove(thisRows.ToArray(), new int[] { column }, new int[] { number }, SudokuPattern.PointingCandidate, possible));
					return true;
				}
			}

			return false;
		}

		private void PointingCandidateConfinement(int row, int column, int startRow, int startColumn, int number, out bool confinedToRow, out bool confinedToColumn)
		{
			confinedToRow = confinedToColumn = true;

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (currentRow == row && currentColumn == column || this.Sudoku[currentRow, currentColumn] != 0)
					{
						continue;
					}

					if (this.Sudoku.ContainsPossible(currentRow, currentColumn, number))
					{
						if (currentRow != row)
						{
							confinedToRow = false;
						}

						if (currentColumn != column)
						{
							confinedToColumn = false;
						}

						if (!(confinedToColumn || confinedToRow))
						{
							return;
						}
					}
				}
			}
		}
		
		private bool ClaimingCandidate(int row, int column, int[] possible)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			return this.ClaimingCandidateRow(row, column, startColumn, possible) || this.ClaimingCandidateColumn(row, column, startRow, possible);
		}

		private bool ClaimingCandidateColumn(int row, int column, int startRow, int[] possible)
		{
			foreach (int number in possible)
			{
				if (this.CheckMoveMade(row, column, number, SudokuPattern.ClaimingCandidate))
				{
					continue;
				}

				bool outside = false;

				for (int i = 0; i < this.Sudoku.Size; i ++)
				{
					if (i == startRow)
					{
						i += this.Sudoku.BlockSize - 1;
						continue;
					}

					if (this.Sudoku[i, column] != 0)
					{
						continue;
					}

					if (this.Sudoku.ContainsPossible(i, column, number))
					{
						outside = true;
						break;
					}
				}

				if (!outside)
				{
					List<int> rows = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentRow = startRow + i;

						if (this.Sudoku.ContainsPossible(currentRow, column, number))
						{
							rows.Add(currentRow);
						}
					}

					this.Moves.Add(new SudokuMove(rows.ToArray(), new int[] { column }, new int[] { number }, SudokuPattern.ClaimingCandidate, possible));
					return true;
				}
			}

			return false;
		}

		private bool ClaimingCandidateRow(int row, int column, int startColumn, int[] possible)
		{
			foreach (int number in possible)
			{
				if (this.CheckMoveMade(row, column, number, SudokuPattern.ClaimingCandidate))
				{
					continue;
				}

				bool outside = false;

				for (int i = 0; i < this.Sudoku.Size; i ++)
				{
					if (i == startColumn)
					{
						i += this.Sudoku.BlockSize - 1;
						continue;
					}

					if (this.Sudoku[row, i] != 0)
					{
						continue;
					}

					if (this.Sudoku.ContainsPossible(row, i, number))
					{
						outside = true;
						break;
					}
				}

				if (!outside)
				{
					List<int> columns = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentColumn = startColumn + i;

						if (this.Sudoku.ContainsPossible(row, currentColumn, number))
						{
							columns.Add(currentColumn);
						}
					}

					this.Moves.Add(new SudokuMove(new int[] { row }, columns.ToArray(), new int[] { number }, SudokuPattern.ClaimingCandidate, possible));
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
