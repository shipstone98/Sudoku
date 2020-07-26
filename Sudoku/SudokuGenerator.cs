using System;
using System.Collections.Generic;

namespace Sudoku
{
	public static class SudokuGenerator
	{
		private static readonly Random Random = new Random();

		public static void AddNumbers(Sudoku sudoku)
		{
			if (sudoku is null)
			{
				throw new NotImplementedException(nameof (sudoku));
			}

			sudoku.DisablePossible = true;
			SudokuSolver.RecursiveSolve(sudoku);
			sudoku.DisablePossible = false;
			sudoku.ResetPossible();
		}

		public static Sudoku AddNumbers(int size, SudokuDifficulty difficulty)
		{
			if (difficulty == SudokuDifficulty.None)
			{
				throw new ArgumentException(nameof(difficulty));
			}

			Sudoku sudoku = new Sudoku(size, difficulty);
			SudokuGenerator.AddNumbers(sudoku);
			return sudoku;
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
				throw new NotImplementedException(nameof(sudoku));
			}

			int attempts = 45;

			switch (sudoku.Difficulty)
			{
				case SudokuDifficulty.Medium:
					attempts = 60;
					break;
				case SudokuDifficulty.Hard:
					attempts = 80;
					break;
			}

			do
			{
				int row, column;

				do
				{
					row = SudokuGenerator.Random.Next(sudoku.Size);
					column = SudokuGenerator.Random.Next(sudoku.Size);
				} while (sudoku[row, column] == 0);

				int value = sudoku[row, column];
				sudoku[row, column] = 0;
				Sudoku clone = (Sudoku) sudoku.Clone();

				if (SudokuSolver.FindSolutions(clone) != 1)
				{
					sudoku[row, column] = value;
				}
			} while (attempts -- > 0);
		}

		internal static void ShuffleNumbers<T>(List<T> array)
		{
			for (int i = 0; i < array.Count - 1; i++)
			{
				int j = SudokuGenerator.Random.Next(i, array.Count);
				T temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
		}
	}
}
