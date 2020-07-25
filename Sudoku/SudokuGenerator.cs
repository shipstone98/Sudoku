using System;
using System.Collections.Generic;

namespace Sudoku
{
	public static class SudokuGenerator
	{
		private static readonly Random Random = new Random();

		public static Sudoku AddNumbers(int size, SudokuDifficulty difficulty)
		{
			if (difficulty == SudokuDifficulty.None)
			{
				throw new ArgumentException(nameof (difficulty));
			}

			Sudoku sudoku = new Sudoku(size, difficulty);
			SudokuGenerator.FillNumbers(sudoku, 0, 0);
			return sudoku;
		}

		private static bool FillNumbers(Sudoku sudoku, int row, int column)
		{
			List<int> possible = new List<int>(sudoku.GetPossible(row, column));
			SudokuGenerator.ShuffleNumbers(possible);

			while (possible.Count != 0)
			{
				sudoku[row, column] = possible[0];
				int nextColumn = column + 1, nextRow = row;

				if (nextColumn == sudoku.Size)
				{
					nextRow ++;
					nextColumn = 0;
				}

				if (nextRow == sudoku.Size || SudokuGenerator.FillNumbers(sudoku, nextRow, nextColumn))
				{
					return true;
				}

				possible.RemoveAt(0);
			}

			sudoku[row, column] = 0;
			return false;
		}

		public static Sudoku Generate(int size, SudokuDifficulty difficulty)
		{
			Sudoku sudoku = SudokuGenerator.AddNumbers(size, difficulty);
			SudokuGenerator.RemoveNumbers(sudoku);
			return sudoku;
		}

		public static void RemoveNumbers(Sudoku sudoku)
		{
			if (sudoku is null)
			{
				throw new NotImplementedException(nameof (sudoku));
			}

			throw new NotImplementedException();
		}

		private static void ShuffleNumbers<T>(List<T> array)
		{
			for (int i = 0; i < array.Count - 1; i ++)
			{
				int j = SudokuGenerator.Random.Next(i, array.Count);
				T temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
		}
	}
}
