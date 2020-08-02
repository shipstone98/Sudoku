using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

		internal static bool Compare<T>(IEnumerable<T> a, IEnumerable<T> b)
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

			SudokuSolver.ShuffleNumbers(possible);

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

		private static void ShuffleNumbers<T>(List<T> array)
		{
			for (int i = 0; i < array.Count - 1; i++)
			{
				int j = SudokuGenerator.Random.Next(i, array.Count);
				T temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
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
			return this.SolveNakedSingle(row, column, possible) || this.SolveHiddenSingle(row, column, possible) || this.PointingCandidate(row, column, possible) || this.ClaimingCandidate(row, column, possible) || this.NakedPair(row, column, possible) || this.NakedTriple(row, column, possible);
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
					int count = 0;
					//Delete all numbers in that row outside the block
					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startColumn)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						if (this.Sudoku.RemovePossible(row, i, number))
						{
							count ++;
						}
					}

					if (count > 0)
					{
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
				}

				if (confinedToColumn && !confinedToRow)
				{
					int count = 0;

					//Delete all numbers in that row outside the block
					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startRow)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						if (this.Sudoku.RemovePossible(i, column, number))
						{
							count ++;
						}
					}

					if (count > 0)
					{
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
			return this.ClaimingCandidateRow(row, column, startRow, startColumn, possible) || this.ClaimingCandidateColumn(row, column, startRow, startColumn, possible);
		}

		private bool ClaimingCandidateColumn(int row, int column, int startRow, int startColumn, int[] possible)
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
					int count = 0;

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentColumn = startColumn + i;

						if (column == currentColumn)
						{
							continue;
						}

						for (int j = 0; j < this.Sudoku.BlockSize; j ++)
						{
							if (this.Sudoku.RemovePossible(startRow + j, currentColumn, number))
							{
								count ++;
							}
						}
					}

					if (count > 0)
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
			}

			return false;
		}

		private bool ClaimingCandidateRow(int row, int column, int startRow, int startColumn, int[] possible)
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
					int count = 0;

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentRow = startRow + i;

						if (row == currentRow)
						{
							continue;
						}

						for (int j = 0; j < this.Sudoku.BlockSize; j ++)
						{
							if (this.Sudoku.RemovePossible(currentRow, startColumn + j, number))
							{
								count ++;
							}
						}
					}

					if (count > 0)
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
			}

			return false;
		}

		private bool NakedPair(int row, int column, int[] possible) => this.NakedPairRow(row, column, possible) || this.NakedPairColumn(row, column, possible) || this.NakedPairBlock(row, column, possible);

		private bool NakedPairBlock(int row, int column, int[] possible)
		{
			int firstRow = -1, firstColumn = -1;
			int[] currentPossible = null;
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (this.Sudoku[currentRow, currentColumn] != 0)
					{
						continue;
					}

					currentPossible = row == currentRow && column == currentColumn ? possible : this.Sudoku.GetPossible(currentRow, currentColumn);

					if (currentPossible.Length == 2)
					{
						firstRow = currentRow;
						firstColumn = currentColumn;
						goto foundFirst;
					}
				}
			}

		foundFirst:
			if (firstRow == -1 || firstColumn == -1 || firstRow == startRow + this.Sudoku.BlockSize - 1 && firstColumn == startColumn + this.Sudoku.BlockSize - 1)
			{
				return false;
			}

			int secondRow = -1, secondColumn = -1;

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (this.Sudoku[currentRow, currentColumn] != 0 || currentRow <= firstRow && currentColumn <= firstColumn)
					{
						continue;
					}

					bool allMatch = true;

					foreach (int number in currentRow == row && currentColumn == column ? possible : this.Sudoku.GetPossible(currentRow, currentColumn))
					{
						if (!currentPossible.Contains(number))
						{
							allMatch = false;
							break;
						}
					}

					if (allMatch)
					{
						secondRow = currentRow;
						secondColumn = currentColumn;
						goto foundSecond;
					}
				}
			}

		foundSecond:
			if (secondRow == -1 || secondColumn == -1)
			{
				return false;
			}

			int count = 0;

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (currentRow == firstRow && currentColumn == firstColumn || currentRow == secondRow && currentColumn == secondColumn)
					{
						continue;
					}

					foreach (int number in currentPossible)
					{
						if (this.Sudoku.RemovePossible(currentRow, currentColumn, number))
						{
							count ++;
						}
					}
				}
			}

			if (count == 0)
			{
				return false;
			}

			SudokuMove move = new SudokuMove(new int[] { firstRow, secondRow }, new int[] { firstColumn, secondColumn }, currentPossible, SudokuPattern.NakedPair, currentPossible);

			if (this.Moves.Contains(move))
			{
				return false;
			}

			this.Moves.Add(move);
			return true;
		}

		private bool NakedPairColumn(int row, int column, int[] possible)
		{
			int currentRow = -1;
			int[] currentPossible = null;

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (this.Sudoku[i, column] != 0)
				{
					continue;
				}

				currentPossible = i == row ? possible : this.Sudoku.GetPossible(i, column);

				if (currentPossible.Length == 2)
				{
					currentRow = i;
					break;
				}
			}

			if (currentRow == -1 || currentRow == this.Sudoku.Size - 1)
			{
				return false;
			}

			int matchingRow = -1;

			for (int i = currentRow + 1; i < this.Sudoku.Size; i ++)
			{
				if (this.Sudoku[i, column] != 0)
				{
					continue;
				}

				bool allMatch = true;

				foreach (int number in i == row ? possible : this.Sudoku.GetPossible(i, column))
				{
					if (!currentPossible.Contains(number))
					{
						allMatch = false;
						break;
					}
				}

				if (allMatch)
				{
					matchingRow = i;
					break;
				}
			}

			if (matchingRow == -1)
			{
				return false;
			}

			int count = 0;

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (i == currentRow || i == matchingRow)
				{
					continue;
				}

				foreach (int number in currentPossible)
				{
					if (this.Sudoku.RemovePossible(i, column, number))
					{
						count ++;
					}
				}
			}

			if (count == 0)
			{
				return false;
			}

			SudokuMove move = new SudokuMove(new int[] { currentRow, matchingRow }, new int[] { column }, currentPossible, SudokuPattern.NakedPair, currentPossible);

			if (this.Moves.Contains(move))
			{
				return false;
			}

			this.Moves.Add(move);
			return true;
		}

		private bool NakedPairRow(int row, int column, int[] possible)
		{
			int currentColumn = -1;
			int[] currentPossible = null;

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (this.Sudoku[row, i] != 0)
				{
					continue;
				}

				currentPossible = i == column ? possible : this.Sudoku.GetPossible(row, i);

				if (currentPossible.Length == 2)
				{
					currentColumn = i;
					break;
				}
			}

			if (currentColumn == -1 || currentColumn == this.Sudoku.Size - 1)
			{
				return false;
			}

			int matchingColumn = -1;

			for (int i = currentColumn + 1; i < this.Sudoku.Size; i ++)
			{
				if (this.Sudoku[row, i] != 0)
				{
					continue;
				}

				bool allMatch = true;

				foreach (int number in i == column ? possible : this.Sudoku.GetPossible(row, i))
				{
					if (!currentPossible.Contains(number))
					{
						allMatch = false;
						break;
					}
				}

				if (allMatch)
				{
					matchingColumn = i;
				}
			}

			if (matchingColumn == -1)
			{
				return false;
			}

			int count = 0;

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				if (i == currentColumn || i == matchingColumn)
				{
					continue;
				}

				foreach (int number in currentPossible)
				{
					if (this.Sudoku.RemovePossible(row, i, number))
					{
						count ++;
					}
				}
			}

			if (count == 0)
			{
				return false;
			}

			SudokuMove move = new SudokuMove(new int[] { row }, new int[] { currentColumn, matchingColumn }, currentPossible, SudokuPattern.NakedPair, currentPossible);

			if (this.Moves.Contains(move))
			{
				return false;
			}

			this.Moves.Add(move);
			return true;
		}

		private bool NakedTriple(int row, int column, int[] possible) => this.NakedTripleBlock(row, column);

		private bool NakedTripleBlock(int row, int column)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					int currentColumn = startColumn + j;

					if (this.Sudoku[currentRow, currentColumn] != 0)
					{
						continue;
					}

					int[] possible = this.Sudoku.GetPossible(currentRow, currentColumn);

					if (possible.Length != 3)
					{
						continue;
					}

					List<Tuple<int, int>> matches = new List<Tuple<int, int>>();
					matches.Add(new Tuple<int, int>(currentRow, currentColumn));

					for (int x = 0; x < this.Sudoku.BlockSize; x ++)
					{
						int nextRow = startRow + x;

						for (int y = 0; y < this.Sudoku.BlockSize; y ++)
						{
							int nextColumn = startColumn + y;

							if (i == x && j == y || this.Sudoku[nextRow, nextColumn] != 0)
							{
								continue;
							}

							int[] nextPossible = this.Sudoku.GetPossible(nextRow, nextColumn);

							if (nextPossible.Length > 3)
							{
								continue;
							}

							bool allMatch = true;

							foreach (int number in nextPossible)
							{
								if (!possible.Contains(number))
								{
									allMatch = false;
									break;
								}
							}

							if (allMatch)
							{
								matches.Add(new Tuple<int, int>(nextRow, nextColumn));
							}
						}
					}

					if (matches.Count != 3)
					{
						continue;
					}

					Tuple<int, int>[] matchesArray = matches.ToArray();
					List<int> rows = new List<int>();
					List<int> columns = new List<int>();

					foreach (Tuple<int, int> match in matchesArray)
					{
						rows.Add(match.Item1);
						columns.Add(match.Item2);
					}

					int count = 0;

					for (int x = 0; x < this.Sudoku.BlockSize; x ++)
					{
						int nextRow = startRow + x;

						for (int y = 0; y < this.Sudoku.BlockSize; y ++)
						{
							int nextColumn = startColumn + y;

							if (matchesArray.Contains(new Tuple<int, int>(nextRow, nextColumn)))
							{
								continue;
							}

							foreach (int number in possible)
							{
								if (this.Sudoku.RemovePossible(nextRow, nextColumn, number))
								{
									count ++;
								}
							}
						}
					}

					if (count == 0)
					{
						return false;
					}

					SudokuMove move = new SudokuMove(rows.ToArray(), columns.ToArray(), possible, SudokuPattern.NakedTriple, possible);

					if (this.Moves.Contains(move))
					{
						return false;
					}

					this.Moves.Add(move);
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
