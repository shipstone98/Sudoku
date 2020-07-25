using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuTest
	{
		public const SudokuDifficulty Difficulty = SudokuDifficulty.Easy;
		public const int Size = 9;

		private int[] Possible { get; }
		private Random Random { get; }
		private Sudoku Sudoku { get; set; }

		public SudokuTest()
		{
			this.Possible = new int[SudokuTest.Size];

			for (int i = 0; i < SudokuTest.Size; i++)
			{
				this.Possible[i] = i + 1;
			}

			this.Random = new Random();
			this.Sudoku = null;
		}

		[TestInitialize]
		public void TestInitialize() => this.Sudoku = new Sudoku(SudokuTest.Size, SudokuTest.Difficulty);

		private static bool ArraysEqual<T>(T[] a, T[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}

			for (int i = 0; i < a.Length; i ++)
			{
				if (!(b.Contains(a[i]) && a.Contains(b[i])))
				{
					return false;
				}
			}

			return true;
		}

		private int Generate(bool includeUpperLimit = false) => includeUpperLimit ? this.Random.Next(SudokuTest.Size + 1) : this.Random.Next(SudokuTest.Size);

		[TestMethod]
		public void TestSetNumberFromZeroToNumber()
		{
			int newNumber = this.Generate(true), row = this.Generate(), column = this.Generate();
			List<int> affectedPossibleList = new List<int>(this.Possible);
			affectedPossibleList.Remove(newNumber);
			int[] affectedPossible = affectedPossibleList.ToArray();
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			this.Sudoku[row, column] = newNumber;

			for (int i = 0; i < SudokuTest.Size; i ++)
			{
				for (int j = 0; j < SudokuTest.Size; j ++)
				{
					this.Sudoku.GetStartRowColumn(i, j, out int currentStartRow, out int currentStartColumn);

					if (i == row || j == column || currentStartRow == startRow && currentStartColumn == startColumn)
					{
						//Check against affectedPossible
						Assert.IsTrue(SudokuTest.ArraysEqual<int>(affectedPossible, this.Sudoku.GetPossible(i, j)));
					}

					else
					{
						Assert.IsTrue(SudokuTest.ArraysEqual<int>(this.Possible, this.Sudoku.GetPossible(i, j)));
					}
				}
			}
		}

		[TestMethod]
		public void TestConstructor()
		{
			Assert.IsNotNull(this.Sudoku);
			Assert.AreEqual(this.Sudoku.Size, SudokuTest.Size);
			Assert.AreEqual(this.Sudoku.Difficulty, SudokuTest.Difficulty);

			for (int i = 0; i < SudokuTest.Size; i ++)
			{
				for (int j = 0; j < SudokuTest.Size; j ++)
				{
					Assert.IsTrue(SudokuTest.ArraysEqual<int>(this.Possible, this.Sudoku.GetPossible(i, j)));
				}
			}
		}
	}
}
