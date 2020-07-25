using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuGeneratorTest
	{
		[TestMethod]
		public void TestAddNumbers()
		{
			Sudoku sudoku = SudokuGenerator.AddNumbers(SudokuTest.Size, SudokuTest.Difficulty);

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
	}
}
