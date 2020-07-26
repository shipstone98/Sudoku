using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
	public class SudokuSolver
	{
		private List<Tuple<int, int, int>> ClaimingCandidates { get; }
		private List<Tuple<int, int>> EmptyCells { get; }
		private List<SudokuMove> Moves { get; }
		private List<Tuple<int, int, int>> PointingCandidates { get; }
		private Random Random { get; }
		public bool Solved { get; private set; }
		public Sudoku Sudoku { get; }

		public SudokuSolver(Sudoku sudoku)
		{
			this.ClaimingCandidates = new List<Tuple<int, int, int>>();
			this.EmptyCells = new List<Tuple<int, int>>();
			this.Moves = new List<SudokuMove>();
			this.PointingCandidates = new List<Tuple<int, int, int>>();
			this.Random = new Random();
			this.Solved = false;
			this.Sudoku = sudoku ?? throw new ArgumentNullException(nameof (sudoku));

			for (int i = 0; i < sudoku.Size; i ++)
			{
				for (int j = 0; j < sudoku.Size; j ++)
				{
					if (sudoku[i, j] == 0)
					{
						this.EmptyCells.Add(new Tuple<int, int>(i, j));
					}
				}
			}
		}

		internal static int FindSolutions(Sudoku sudoku)
		{
			int count = 0;
			SudokuSolver.RecursiveSolve(sudoku, 0, 0, ref count, true);
			return count;
		}

		public static void RecursiveSolve(Sudoku sudoku)
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

		private static bool RecursiveSolve(Sudoku sudoku, int row, int column, ref int count, bool findMultiple = false)
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

		private static bool RecursiveSolveNextNumber(Sudoku sudoku, int row, int column, ref int count, bool findMultiple = false)
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

		public IEnumerable<SudokuMove> Solve()
		{
			if (!this.Solved)
			{
				while (this.EmptyCells.Count > 0)
				{
					if (!this.SolvePass())
					{
						break;
					}
				}

				this.Solved = true;
			}

			return this.Moves;
		}

		private bool SolveClaimingCandidate(int row, int column)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			int[] possible = this.Sudoku.GetPossible(row, column);

			foreach (int number in possible)
			{
				if (this.ClaimingCandidates.Any(item => item.Item1 == row && item.Item2 == column && item.Item3 == number))
				{
					continue;
				}

				if (this.SolveClaimingCandidateRow(row, column, startColumn, number, possible))
				{
					List<int> affected = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentColumn = startColumn + i;

						if (currentColumn == column || !(this.Sudoku[row, currentColumn] == 0 && this.Sudoku.ContainsPossible(row, currentColumn, number)))
						{
							continue;
						}

						affected.Add(currentColumn);
						this.ClaimingCandidates.Add(new Tuple<int, int, int>(row, i, number));
					}

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentRow = startRow + i;

						if (currentRow == row)
						{
							continue;
						}

						for (int j = 0; j < this.Sudoku.BlockSize; j ++)
						{
							int currentColumn = startColumn + j;
							this.Sudoku.RemovePossible(currentRow, currentColumn, number);
						}
					}

					this.Moves.Add(new SudokuPossibleMove(row, column, number, SudokuPattern.ClaimingCandidate, possible, new int[] { row }, affected.ToArray(), null));
					return true;
				}

				if (this.SolveClaimingCandidateColumn(row, column, startRow, number, possible))
				{
					List<int> affected = new List<int>();

					for (int i = 0; i < this.Sudoku.BlockSize; i ++)
					{
						int currentRow = startRow + i;

						if (currentRow == row || !(this.Sudoku[currentRow, column] == 0 && this.Sudoku.ContainsPossible(currentRow, column, number)))
						{
							continue;
						}

						affected.Add(currentRow);
						this.ClaimingCandidates.Add(new Tuple<int, int, int>(i, column, number));
					}

					for (int j = 0; j < this.Sudoku.BlockSize; j ++)
					{
						int currentColumn = startColumn + j;

						if (currentColumn == column)
						{
							continue;
						}

						for (int i = 0; i < this.Sudoku.BlockSize; i ++)
						{
							int currentRow = startRow + j;
							this.Sudoku.RemovePossible(currentRow, currentColumn, number);
						}
					}

					this.Moves.Add(new SudokuPossibleMove(row, column, number, SudokuPattern.ClaimingCandidate, possible, affected.ToArray(), new int[] { column }, null));
					return true;
				}
			}

			return false;
		}

		private bool SolveClaimingCandidateColumn(int row, int column, int startRow, int number, int[] possible)
		{
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
					return false;
				}
			}

			return true;
		}

		private bool SolveClaimingCandidateRow(int row, int column, int startColumn, int number, int[] possible)
		{
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
					return false;
				}
			}

			return true;
		}

		private bool SolveHiddenSingle(int row, int column)
		{
			if (row == 0 && column == 6)
			{
				int x = 3;
			}

			int[] possible = this.Sudoku.GetPossible(row, column);
			
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

		private bool SolveNakedSingle(int row, int column)
		{
			int[] possible = this.Sudoku.GetPossible(row, column);

			if (possible.Length == 1 && this.Sudoku[row, column] == 0)
			{
				this.Sudoku[row, column] = possible[0];
				this.Moves.Add(new SudokuMove(row, column, possible[0], SudokuPattern.NakedSingle, possible));
				return true;
			}

			return false;
		}

		private bool SolvePointingCandidate(int row, int column)
		{
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			int[] possible = this.Sudoku.GetPossible(row, column);
			List<int> rowsAffected = new List<int>(), columnsAffected = new List<int>();

			foreach (int number in possible)
			{
				bool confinedToRow = true, confinedToColumn = true;
				rowsAffected.Clear();
				columnsAffected.Clear();

				for (int i = 0; i < this.Sudoku.BlockSize; i ++)
				{
					int currentRow = startRow + i;

					for (int j = 0; j < this.Sudoku.BlockSize; j ++)
					{
						int currentColumn = startColumn + j;

						if (currentRow == row && currentColumn == column || this.Sudoku[currentRow, currentColumn] != 0 || this.PointingCandidates.Any(item => item.Item1 == currentRow && item.Item2 == currentColumn && item.Item3 == number))
						{
							continue;
						}

						if (this.Sudoku.ContainsPossible(currentRow, currentColumn, number))
						{
							if (row != currentRow)
							{
								confinedToRow = false;

								if (!rowsAffected.Contains(currentRow))
								{
									rowsAffected.Add(currentRow);
								}
							}

							if (column != currentColumn)
							{
								confinedToColumn = false;

								if (!columnsAffected.Contains(currentColumn))
								{
									columnsAffected.Add(currentColumn);
								}
							}
						}
					}
				}

				if (confinedToRow && !confinedToColumn)
				{
					int[] columns = columnsAffected.ToArray();

					foreach (int c in columns)
					{
						this.PointingCandidates.Add(new Tuple<int, int, int>(row, c, number));
					}

					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startColumn)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						this.Sudoku.RemovePossible(row, i, number);
					}

					this.Moves.Add(new SudokuPossibleMove(row, column, number, SudokuPattern.PointingCandidate, possible, null, columns, null));
					return true;
				}

				else if (!confinedToRow && confinedToColumn)
				{
					int[] rows = rowsAffected.ToArray();

					foreach (int r in rows)
					{
						this.PointingCandidates.Add(new Tuple<int, int, int>(r, column, number));
					}

					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						if (i == startRow)
						{
							i += this.Sudoku.BlockSize - 1;
							continue;
						}

						this.Sudoku.RemovePossible(i, column, number);
					}

					this.Moves.Add(new SudokuPossibleMove(row, column, number, SudokuPattern.PointingCandidate, possible, rows, null, null));
					return true;
				}
			}

			return false;
		}

		private bool SolvePass()
		{
			List<Tuple<int, int>> remainingCells = new List<Tuple<int, int>>(this.EmptyCells);

			while (remainingCells.Count > 0)
			{
				int index = this.Random.Next(remainingCells.Count);
				Tuple<int, int> selected = remainingCells[index];

				if (this.SolvePass(selected.Item1, selected.Item2))
				{
					this.EmptyCells.Remove(selected);
					return true;
				}

				remainingCells.RemoveAt(index);
			}

			return false;
		}

		private bool SolvePass(int row, int column) => this.SolveNakedSingle(row, column) || this.SolveHiddenSingle(row, column) || this.SolvePointingCandidate(row, column) || this.SolveClaimingCandidate(row, column);
	}
}
