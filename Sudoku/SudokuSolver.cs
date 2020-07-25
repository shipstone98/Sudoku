using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
	public class SudokuSolver
	{
		public Sudoku Sudoku { get; }

		public SudokuSolver(Sudoku sudoku) => this.Sudoku = sudoku ?? throw new ArgumentNullException(nameof (sudoku));

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

		public Object Solve() => throw new NotImplementedException();
	}
}
