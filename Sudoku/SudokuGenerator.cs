using System;
using System.Collections.Generic;

namespace Sudoku
{
	/// <summary>
	/// Generates <see cref="Sudoku"/> puzzles.
	/// </summary>
	public static class SudokuGenerator
	{
		private static readonly Random Random = new Random();

		/// <summary>
		/// Fills an existing <see cref="Sudoku"/> puzzle with numbers.
		/// </summary>
		/// <param name="sudoku">The puzzle to fill with numbers.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
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
			sudoku.SetSolutions();
		}

		/// <summary>
		/// Creates a new <see cref="Sudoku"/> puzzle and fills it with numbers.
		/// </summary>
		/// <param name="size">The number of elements that the new <see cref="Sudoku"/> puzzle can store in each row, column and block.</param>
		/// <param name="difficulty">The difficulty associated with the <see cref="Sudoku"/> puzzle.</param>
		/// <returns>A new <see cref="Sudoku"/> puzzle filled with numbers.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer - or - <c><paramref name="difficulty"/></c> is equal to <see cref="SudokuDifficulty.None"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0 or greater than <see cref="Sudoku.MaximumSupportedSize"/>.</exception>
		public static Sudoku AddNumbers(int size, SudokuDifficulty difficulty)
		{
			if (difficulty == SudokuDifficulty.None)
			{
				throw new ArgumentException(nameof (difficulty));
			}

			Sudoku sudoku = new Sudoku(size, difficulty);
			SudokuGenerator.AddNumbers(sudoku);
			return sudoku;
		}

		/// <summary>
		/// Generates a new <see cref="Sudoku"/> puzzle by filling it with numbers and then removing numbers according to <c><paramref name="difficulty"/></c>.
		/// </summary>
		/// <param name="size">The number of elements that the new <see cref="Sudoku"/> puzzle can store in each row, column and block.</param>
		/// <param name="difficulty">The difficulty associated with the <see cref="Sudoku"/> puzzle.</param>
		/// <returns>A new <see cref="Sudoku"/> puzzle.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer - or - <c><paramref name="difficulty"/></c> is equal to <see cref="SudokuDifficulty.None"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0 or greater than <see cref="Sudoku.MaximumSupportedSize"/>.</exception>
		public static Sudoku Generate(int size, SudokuDifficulty difficulty)
		{
			Sudoku sudoku = SudokuGenerator.AddNumbers(size, difficulty);
			SudokuGenerator.RemoveNumbers(sudoku);
			return sudoku;
		}

		/// <summary>
		/// Removes numbers from an existing <see cref="Sudoku"/> puzzles according to its difficulty.
		/// </summary>
		/// <param name="sudoku">The <see cref="Sudoku"/> puzzles to remove numbers from.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static void RemoveNumbers(Sudoku sudoku)
		{
			if (sudoku is null)
			{
				throw new NotImplementedException(nameof (sudoku));
			}

			int attempts = 45;

			switch (sudoku.Difficulty)
			{
				case SudokuDifficulty.Medium:
					attempts = 60;
					break;
				case SudokuDifficulty.Hard:
					attempts = 75;
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
				bool solvable = SudokuSolver.CheckSolvable(sudoku, out bool multipleSolutions);

				if (!solvable || multipleSolutions)
				{
					sudoku[row, column] = value;
				}
			} while (attempts -- > 0);

			sudoku.SetReadOnlyProperties();
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
