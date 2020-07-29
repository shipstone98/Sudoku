using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuSolverTest
	{
		[TestMethod]
		public void TestRecursiveSolve()
		{
			const String SUDOKU_FILENAME = "Sudoku.txt";
			String sudokuText;

			try
			{
				sudokuText = File.ReadAllText(SUDOKU_FILENAME);
			}

			catch
			{
				Assert.Fail($"A sudoku puzzle must be exist in the current directory. The file must be called \"{SUDOKU_FILENAME}\".");
				return;
			}

			const String SOLUTION_FILENAME = "Solution.txt";
			String solutionText;

			try
			{
				solutionText = File.ReadAllText(SOLUTION_FILENAME);
			}

			catch
			{
				Assert.Fail($"A sudoku solution must be exist in the current directory. The file must be called \"{SOLUTION_FILENAME}\".");
				return;
			}

			SudokuPuzzle sudoku;

			try
			{
				sudoku = SudokuPuzzle.Parse(sudokuText);
			}

			catch (FormatException)
			{
				Assert.Fail($"The sudoku puzzle specified in the file {SUDOKU_FILENAME} was not in the correct format");
				return;
			}

			SudokuPuzzle solution;

			try
			{
				solution = SudokuPuzzle.Parse(solutionText);
			}

			catch (FormatException)
			{
				Assert.Fail($"The sudoku solution specified in the file {SOLUTION_FILENAME} was not in the correct format");
				return;
			}

			SudokuSolver.RecursiveSolve(sudoku);
			Assert.AreEqual(sudoku, solution);
		}
	}
}
