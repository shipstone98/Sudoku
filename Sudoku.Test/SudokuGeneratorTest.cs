using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuGeneratorTest
	{
		[TestMethod]
		public void TestAddNumbers()
		{
			SudokuPuzzle sudoku = SudokuGenerator.AddNumbers(SudokuPuzzleTest.Size, SudokuPuzzleTest.Difficulty);
			Assert.ThrowsException<ArgumentNullException>(() => SudokuGenerator.AddNumbers(null));
			Assert.ThrowsException<ArgumentException>(() => SudokuGenerator.AddNumbers(SudokuPuzzle.MaximumSupportedSize, SudokuDifficulty.None));

			if (SudokuPuzzle.MaximumSupportedSize > 1)
			{
				Assert.ThrowsException<ArgumentException>(() => SudokuGenerator.AddNumbers(2, SudokuDifficulty.Easy));
			}

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => SudokuGenerator.AddNumbers(SudokuPuzzle.MaximumSupportedSize + 1, SudokuDifficulty.Easy));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => SudokuGenerator.AddNumbers(-1, SudokuDifficulty.Easy));

			for (int i = 0; i < sudoku.Size; i ++)
			{
				for (int j = 0; j < sudoku.Size; j ++)
				{
					int number = sudoku[i, j];
					Assert.IsFalse(sudoku.RowContains(i, j, number, true));
					Assert.IsFalse(sudoku.ColumnContains(i, j, number, true));
					Assert.IsFalse(sudoku.BlockContains(i, j, number, true));
				}
			}
		}

		[TestMethod]
		public void TestRemoveNumbers()
		{
			Assert.ThrowsException<ArgumentNullException>(() => SudokuGenerator.RemoveNumbers(null));
			SudokuPuzzle sudoku = SudokuGenerator.AddNumbers(SudokuPuzzleTest.Size, SudokuPuzzleTest.Difficulty);
			Console.WriteLine(sudoku);
			SudokuPuzzle original = (SudokuPuzzle) sudoku.Clone();
			SudokuGenerator.RemoveNumbers(sudoku);
			Console.WriteLine(sudoku);
			SudokuSolver.RecursiveSolve(sudoku);
			Assert.AreEqual(original, sudoku);
		}
	}
}
