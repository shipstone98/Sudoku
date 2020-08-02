using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuCellTest
	{
		private SudokuCell Cell;

		[TestInitialize]
		public void Initialize()
		{
			this.Cell = new SudokuCell();
		}

		[TestMethod]
		public void TestIsCorrect()
		{
			Assert.IsFalse(this.Cell.IsCorrect);
			this.Cell.Solution = SudokuPuzzle.MaximumSupportedSize;
			Assert.IsFalse(this.Cell.IsCorrect);
			this.Cell.Solution = SudokuPuzzle.MaximumSupportedSize - 1;
			Assert.IsFalse(this.Cell.IsCorrect);
			this.Cell.Number = this.Cell.Solution;
			Assert.IsTrue(this.Cell.IsCorrect);
		}
	}
}
