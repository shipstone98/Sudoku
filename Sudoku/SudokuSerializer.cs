using System;
using System.Text;

namespace Sudoku
{
	/// <summary>
	/// Provides functionality to seralize <see cref="SudokuPuzzle"/> instances into a standard format and to deserialize a standard format into a <see cref="SudokuPuzzle"/> instance.
	/// </summary>
	public static class SudokuSerializer
	{
		/// <summary>
		/// Parses the text representing a single <see cref="SudokuPuzzle"/>.
		/// </summary>
		/// <param name="s">The text to parse.</param>
		/// <returns>A <see cref="SudokuPuzzle"/> representation of <c><paramref name="s"/></c>.</returns>
		/// <exception cref="ArgumentNullException"><c><paramref name="s"/></c> is <c>null</c>.</exception>
		/// <exception cref="FormatException"><c><paramref name="s"/></c> is not in the correct format.</exception>
		public static SudokuPuzzle Deserialize(String s)
		{
			if (s is null)
			{
				throw new ArgumentNullException(nameof (s));
			}

			String[] split = s.Split(',');

			if (split.Length != 5)
			{
				throw new FormatException();
			}

			try
			{
				int size = Int32.Parse(split[0]);
				int squared = size * size;

				if (!SudokuPuzzle.VerifySize(size))
				{
					throw new FormatException();
				}

				SudokuDifficulty difficulty = (SudokuDifficulty) Enum.Parse(typeof (SudokuDifficulty), split[1], true);
				int[] numbers = new int[squared], solutions = new int[squared];
				bool[] readOnly = new bool[squared];

				if (split[2].Length != squared || split[3].Length != squared || split[4].Length != squared)
				{
					throw new FormatException();
				}

				for (int i = 0; i < squared; i ++)
				{
					numbers[i] = split[2][i] - '0';
					solutions[i] = split[3][i] - '0';
					readOnly[i] = split[3][i] == '1';
				}

				return SudokuPuzzle.Parse(size, difficulty, numbers, solutions, readOnly);
			}

			catch
			{
				throw new FormatException();
			}
		}

		/// <summary>
		/// Converts a specified <see cref="SudokuPuzzle"/> into a string.
		/// </summary>
		/// <param name="sudoku">The <see cref="SudokuPuzzle"/> to convert.</param>
		/// <returns>The standard format string representation of <c><paramref name="sudoku"/></c>.</returns>
		/// <exception cref="ArgumentNullException"><c><paramref name="sudoku"/></c> is <c>null</c>.</exception>
		public static String Seralize(SudokuPuzzle sudoku)
		{
			if (sudoku is null)
			{
				throw new ArgumentNullException(nameof (sudoku));
			}

			StringBuilder numberBuilder = new StringBuilder();
			StringBuilder solutionBuilder = new StringBuilder();
			StringBuilder readOnlyBuilder = new StringBuilder();

			for (int i = 0; i < sudoku.Size; i ++)
			{
				for (int j = 0; j < sudoku.Size; j ++)
				{
					numberBuilder.Append(sudoku[i, j]);
					solutionBuilder.Append(sudoku.GetSolution(i, j));
					readOnlyBuilder.Append(sudoku.CheckReadOnly(i, j) ? 1 : 0);
				}
			}

			return String.Join(",", sudoku.Size, (int) sudoku.Difficulty, numberBuilder, solutionBuilder, readOnlyBuilder);
		}
	}
}
