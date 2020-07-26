﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
	public class SudokuSolver
	{
		private List<Tuple<int, int>> EmptyCells { get; }
		private List<SudokuMove> Moves { get; }
		private Random Random { get; }
		public bool Solved { get; private set; }
		public Sudoku Sudoku { get; }

		public SudokuSolver(Sudoku sudoku)
		{
			this.EmptyCells = new List<Tuple<int, int>>();
			this.Moves = new List<SudokuMove>();
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
			SudokuSolver.RecursiveSolve(sudoku, 0, 0, ref count);
		}

		private static bool RecursiveSolve(Sudoku sudoku, int row, int column, ref int count, bool findMultiple = false)
		{
			if (sudoku[row, column] != 0)
			{
				return SudokuSolver.RecursiveSolveNextNumber(sudoku, row, column, ref count, findMultiple);
			}

			List<int> possible = new List<int>(sudoku.GetPossible(row, column));
			SudokuGenerator.ShuffleNumbers(possible);

			while (possible.Count != 0)
			{
				sudoku[row, column] = possible[0];

				if (SudokuSolver.RecursiveSolveNextNumber(sudoku, row, column, ref count, findMultiple))
				{
					return true;
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
				nextRow++;
				nextColumn = 0;
			}

			return nextRow == sudoku.Size || SudokuSolver.RecursiveSolve(sudoku, nextRow, nextColumn, ref count, findMultiple);
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

		private bool SolveHiddenSingle(int row, int column)
		{
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

			for (int i = 0; i < this.Sudoku.SizeSqrt; i ++)
			{
				int currentRow = startRow + i;

				for (int j = 0; j < this.Sudoku.SizeSqrt; j ++)
				{
					int currentColumn = startColumn + j;

					if (row == currentRow && column == currentColumn)
					{
						continue;
					}

					if (this.Sudoku.GetPossible(row, column).Contains(number))
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
				if (i == row)
				{
					continue;
				}

				if (this.Sudoku.GetPossible(i, column).Contains(number))
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
				if (i == column)
				{
					continue;
				}

				if (this.Sudoku.GetPossible(row, i).Contains(number))
				{
					return false;
				}
			}

			return true;
		}

		private bool SolveNakedSingle(int row, int column)
		{
			int[] possible = this.Sudoku.GetPossible(row, column);

			if (possible.Length == 1)
			{
				this.Sudoku[row, column] = possible[0];
				this.Moves.Add(new SudokuMove(row, column, possible[0], SudokuPattern.NakedSingle, possible));
				return true;
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

		private bool SolvePass(int row, int column) => this.SolveNakedSingle(row, column) || this.SolveHiddenSingle(row, column);
	}
}
