using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuMoveTest
	{
		private SudokuMove Move;

		[TestInitialize]
		public void Initialize()
		{
			const int NUMBER = SudokuPuzzle.MaximumSupportedSize;
			this.Move = new SudokuMove(NUMBER, NUMBER, NUMBER, SudokuPattern.HiddenSingle, new int[] { NUMBER });
		}

		[TestMethod]
		public void TestConstructor()
		{
			const SudokuPattern BAD_PATTERN = SudokuPattern.None;
			const SudokuPattern GOOD_PATTERN = SudokuPattern.ClaimingCandidate;
			const int BAD_NUMBER = -1;
			const int GOOD_NUMBER = SudokuPuzzle.MaximumSupportedSize;
			int[] badArray = new int[] { -1 };
			int[] emptyArray = new int[0];
			int[] goodArray = new int[] { 1, GOOD_NUMBER };
			Assert.ThrowsException<ArgumentNullException>(() => new SudokuMove(null, null, null, GOOD_PATTERN, null));
			Assert.ThrowsException<ArgumentNullException>(() => new SudokuMove(goodArray, null, null, GOOD_PATTERN, null));
			Assert.ThrowsException<ArgumentNullException>(() => new SudokuMove(goodArray, goodArray, null, GOOD_PATTERN, null));
			Assert.ThrowsException<ArgumentNullException>(() => new SudokuMove(goodArray, goodArray, goodArray, GOOD_PATTERN, null));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(badArray, badArray, badArray, BAD_PATTERN, badArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, badArray, badArray, BAD_PATTERN, badArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, goodArray, badArray, BAD_PATTERN, badArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, goodArray, goodArray, BAD_PATTERN, badArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(emptyArray, emptyArray, emptyArray, BAD_PATTERN, emptyArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, emptyArray, emptyArray, BAD_PATTERN, emptyArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, goodArray, emptyArray, BAD_PATTERN, emptyArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, goodArray, goodArray, BAD_PATTERN, emptyArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(goodArray, goodArray, new int[] { GOOD_NUMBER, 1, 1 }, GOOD_PATTERN, goodArray));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SudokuMove(BAD_NUMBER, BAD_NUMBER, BAD_NUMBER, BAD_PATTERN, goodArray));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SudokuMove(GOOD_NUMBER, BAD_NUMBER, BAD_NUMBER, BAD_PATTERN, goodArray));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SudokuMove(GOOD_NUMBER, GOOD_NUMBER, BAD_NUMBER, BAD_PATTERN, goodArray));
			Assert.ThrowsException<ArgumentException>(() => new SudokuMove(GOOD_NUMBER, GOOD_NUMBER, GOOD_NUMBER, BAD_PATTERN, goodArray));

			try
			{
				new SudokuMove(goodArray, goodArray, goodArray, GOOD_PATTERN, goodArray);
			}

			catch (Exception ex)
			{
				Assert.Fail($"Throw exception: {ex}");
			}
		}

		[TestMethod]
		public void TestProperties()
		{
			int[] array = new int[] { 1, SudokuPuzzle.MaximumSupportedSize };
			SudokuMove move = new SudokuMove(array, array, array, SudokuPattern.HiddenSingle, array);
			Assert.IsTrue(SudokuCellTest.Equal(array, move.Columns));
			Assert.IsTrue(SudokuCellTest.Equal(array, move.Numbers));
			Assert.IsTrue(SudokuCellTest.Equal(array, move.Possible));
			Assert.IsTrue(SudokuCellTest.Equal(array, move.Rows));
		}

		[TestMethod]
		public void TestEquals()
		{
			SudokuMove move = this.Move;
			Assert.IsTrue(this.Move.Equals(move));
			Assert.IsTrue(this.Move == move);
			Assert.IsFalse(this.Move != move);
			Assert.AreEqual(this.Move, move);
			Assert.IsFalse(this.Move.Equals(new Object()));
			Assert.IsFalse(this.Move.Equals(null));
			move = null;
			Assert.IsFalse(this.Move == null);
			Assert.IsFalse(null == this.Move);
			Assert.IsTrue(move == null);
			move = new SudokuMove(1, 1, 1, SudokuPattern.HiddenSingle, new int[] { 1 });
			Assert.IsFalse(this.Move == move);
		}

		[TestMethod]
		public void TestGetHashCode()
		{
			SudokuMove move = this.Move;
			int hashCode = this.Move.GetHashCode();
			Assert.AreEqual(move.GetHashCode(), hashCode);
			move = new SudokuMove(1, 1, 1, SudokuPattern.HiddenSingle, new int[] { 1 });
			Assert.AreNotEqual(move.GetHashCode(), hashCode);
		}
	}
}
