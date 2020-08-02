using System;
using System.Collections.Generic;

namespace Sudoku
{
	/// <summary>
	/// Generates a <see cref="SudokuPuzzle"/>.
	/// </summary>
	public static class SudokuGenerator
	{
		private static readonly Random Random = new Random();

		/// <summary>
		/// Fills an existing <see cref="SudokuPuzzle"/> with numbers.
		/// </summary>
		/// <param name="sudoku">The puzzle to fill with numbers.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static void AddNumbers(SudokuPuzzle sudoku)
		{
			if (sudoku is null)
			{
				throw new ArgumentNullException(nameof (sudoku));
			}

			sudoku.DisablePossible = true;
			SudokuSolver.RecursiveSolve(sudoku);
			sudoku.DisablePossible = false;
			sudoku.ResetPossible();
			sudoku.ClearReadOnlyProperties();
			sudoku.SetSolutions();
		}

		/// <summary>
		/// Creates a new <see cref="SudokuPuzzle"/> and fills it with numbers.
		/// </summary>
		/// <param name="size">The number of elements that the new <see cref="SudokuPuzzle"/> can store in each row, column and block.</param>
		/// <param name="difficulty">The difficulty associated with the <see cref="SudokuPuzzle"/>.</param>
		/// <returns>A new <see cref="SudokuPuzzle"/> filled with numbers.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer - or - <c><paramref name="difficulty"/></c> is equal to <see cref="SudokuDifficulty.None"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0 or greater than <see cref="SudokuPuzzle.MaximumSupportedSize"/>.</exception>
		public static SudokuPuzzle AddNumbers(int size, SudokuDifficulty difficulty)
		{
			if (difficulty == SudokuDifficulty.None)
			{
				throw new ArgumentException(nameof (difficulty));
			}

			SudokuPuzzle sudoku = new SudokuPuzzle(size, difficulty);
			SudokuGenerator.AddNumbers(sudoku);
			return sudoku;
		}

		/// <summary>
		/// Generates a new <see cref="SudokuPuzzle"/> by filling it with numbers and then removing numbers according to <c><paramref name="difficulty"/></c>.
		/// </summary>
		/// <param name="size">The number of elements that the new <see cref="SudokuPuzzle"/> can store in each row, column and block.</param>
		/// <param name="difficulty">The difficulty associated with the <see cref="SudokuPuzzle"/>.</param>
		/// <returns>A new <see cref="SudokuPuzzle"/>.</returns>
		/// <exception cref="ArgumentException"><c><paramref name="size"/></c> is not a positive, square integer - or - <c><paramref name="difficulty"/></c> is equal to <see cref="SudokuDifficulty.None"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><c><paramref name="size"/></c> is less than or equal to 0 or greater than <see cref="SudokuPuzzle.MaximumSupportedSize"/>.</exception>
		public static SudokuPuzzle Generate(int size, SudokuDifficulty difficulty)
		{
			SudokuPuzzle sudoku = SudokuGenerator.AddNumbers(size, difficulty);
			SudokuGenerator.RemoveNumbers(sudoku);
			return sudoku;
		}

		/// <summary>
		/// Removes numbers from an existing <see cref="SudokuPuzzle"/> according to its difficulty.
		/// </summary>
		/// <param name="sudoku">The <see cref="SudokuPuzzle"/> to remove numbers from.</param>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static void RemoveNumbers(SudokuPuzzle sudoku)
		{
			if (sudoku is null)
			{
				throw new ArgumentNullException(nameof (sudoku));
			}

			sudoku.ClearReadOnlyProperties();
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
	}
}
