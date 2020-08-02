using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuCellTest
	{
		private SudokuCell Cell;

		private static bool Equal<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a is null)
			{
				return b is null;
			}

			if (b is null)
			{
				return false;
			}

			int aLength = a.Count(), bLength = b.Count();

			if (aLength != bLength)
			{
				return false;
			}

			foreach (T item in a)
			{
				if (!b.Contains(item))
				{
					return false;
				}
			}

			foreach (T item in b)
			{
				if (!a.Contains(item))
				{
					return false;
				}
			}

			return true;
		}

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

		[TestMethod]
		public void TestAddPossible()
		{
			int[] possible = new int[SudokuPuzzle.MaximumSupportedSize];

			for (int i = 0; i < SudokuPuzzle.MaximumSupportedSize; i ++)
			{
				int number = i + 1;
				possible[i] = number;
				this.Cell.AddPossible(number);
			}

			Assert.IsTrue(SudokuCellTest.Equal(possible, this.Cell.Possible));
			this.Cell.AddPossible(SudokuPuzzle.MaximumSupportedSize);
			Assert.IsTrue(SudokuCellTest.Equal(possible, this.Cell.Possible));
			this.Cell.RemovePossible(SudokuPuzzle.MaximumSupportedSize);
			Assert.IsFalse(SudokuCellTest.Equal(possible, this.Cell.Possible));
			this.Cell.AddPossible(SudokuPuzzle.MaximumSupportedSize);
			Assert.IsTrue(SudokuCellTest.Equal(possible, this.Cell.Possible));
		}

		[TestMethod]
		public void TestClone()
		{
			SudokuCell clone = (SudokuCell) this.Cell.Clone();
			Assert.AreEqual(this.Cell.Number, clone.Number);
			Assert.AreEqual(this.Cell.Solution, clone.Solution);
			Assert.AreEqual(this.Cell.IsReadOnly, clone.IsReadOnly);
			Assert.IsTrue(SudokuCellTest.Equal(this.Cell.Possible, clone.Possible));
		}

		[TestMethod]
		public void TestEquals()
		{
			SudokuCell cell = (SudokuCell) this.Cell.Clone();
			Assert.IsTrue(cell.Equals(this.Cell));
			Assert.IsTrue(cell == this.Cell);
			Assert.IsFalse(cell != this.Cell);
			Assert.AreEqual(cell, this.Cell);
			Assert.IsFalse(this.Cell.Equals(new Object()));
			Assert.IsFalse(this.Cell.Equals(null));
			cell.Number = SudokuPuzzle.MaximumSupportedSize;
			this.Cell.Number = 0;
			Assert.IsFalse(this.Cell.Equals(cell));
		}

		[TestMethod]
		public void TestGetHashCode()
		{
			SudokuCell cell = (SudokuCell) this.Cell.Clone();
			Assert.AreEqual(cell.GetHashCode(), this.Cell.GetHashCode());
			cell.Number = SudokuPuzzle.MaximumSupportedSize;
			this.Cell.Number = 0;
			Assert.AreNotEqual(cell.GetHashCode(), this.Cell.GetHashCode());
			this.Cell.Number = cell.Number;
			Assert.AreEqual(cell.GetHashCode(), this.Cell.GetHashCode());
		}
	}
}
