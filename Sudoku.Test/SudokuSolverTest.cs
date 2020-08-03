using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuSolverTest
	{
		private SudokuPuzzle Sudoku;
		private SudokuPuzzle Solution;

		[TestInitialize]
		public void Initialize()
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

			try
			{
				this.Sudoku = SudokuPuzzle.Parse(sudokuText);
			}

			catch (FormatException)
			{
				Assert.Fail($"The sudoku puzzle specified in the file {SUDOKU_FILENAME} was not in the correct format");
				return;
			}

			try
			{
				this.Solution = SudokuPuzzle.Parse(solutionText);
			}

			catch (FormatException)
			{
				Assert.Fail($"The sudoku solution specified in the file {SOLUTION_FILENAME} was not in the correct format");
				return;
			}

		}

		[TestMethod]
		public void TestRecursiveSolve()
		{
			SudokuSolver.RecursiveSolve(this.Sudoku);
			Assert.AreEqual(this.Sudoku, this.Solution);
		}

		[TestMethod]
		public void TestSolve()
		{
			SudokuSolver solver = new SudokuSolver(this.Sudoku);
			solver.Solve();
			Assert.AreEqual(this.Sudoku, this.Solution);
		}
	}
}
