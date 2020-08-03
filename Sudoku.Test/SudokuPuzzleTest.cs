using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuPuzzleTest
	{
		public const SudokuDifficulty DefaultDifficulty = SudokuDifficulty.Easy;
		public const int DefaultSize = 9;
		private const char SeparatorChar = '=';

		public static SudokuDifficulty Difficulty { get; private set; }
		public static int Size { get; private set; }

		private int[] Possible { get; }
		private Random Random { get; }
		private SudokuPuzzle Sudoku { get; set; }

		static SudokuPuzzleTest()
		{
			try
			{
				if (File.Exists("vars.txt"))
				{
					SudokuPuzzleTest.ParseFile("vars.txt");
				}

				else if (File.Exists("Variables.txt"))
				{
					SudokuPuzzleTest.ParseFile("Variables.txt");
				}

				else
				{
					throw new Exception();
				}
			}

			catch
			{
				SudokuPuzzleTest.Difficulty = SudokuPuzzleTest.DefaultDifficulty;
				SudokuPuzzleTest.Size = SudokuPuzzleTest.DefaultSize;
			}

			Assert.AreNotEqual(SudokuPuzzleTest.Difficulty, SudokuDifficulty.None, "Difficulty must not be None");
			Assert.IsTrue(SudokuPuzzle.VerifySize(SudokuPuzzleTest.Size), "Size must be a positive, square integer");
		}

		public SudokuPuzzleTest()
		{
			this.Possible = new int[SudokuPuzzleTest.Size];

			for (int i = 0; i < SudokuPuzzleTest.Size; i++)
			{
				this.Possible[i] = i + 1;
			}

			this.Random = new Random();
			this.Sudoku = null;
		}

		[TestInitialize]
		public void TestInitialize() => this.Sudoku = new SudokuPuzzle(SudokuPuzzleTest.Size, SudokuPuzzleTest.Difficulty);

		private static void ParseFile(String name)
		{
			String[] lines = File.ReadAllLines(name);

			foreach (String line in lines)
			{
				String formattedLine = line.ToLower();
				formattedLine = Regex.Replace(formattedLine, @"\s+", "");

				if (formattedLine.Count(c => c == SudokuPuzzleTest.SeparatorChar) != 1)
				{
					continue;
				}

				int index = formattedLine.IndexOf(SudokuPuzzleTest.SeparatorChar);
				String key = formattedLine.Substring(0, index);
				String value = formattedLine.Substring(index + 1);

				switch (key)
				{
					case "d":
					case "diff":
					case "difficulty":
						try
						{
							SudokuPuzzleTest.Difficulty = (SudokuDifficulty) Enum.Parse(typeof (SudokuDifficulty), value, true);
						}

						catch
						{
							try
							{
								uint parsedValue = UInt32.Parse(value);
								SudokuPuzzleTest.Difficulty = (SudokuDifficulty) parsedValue;
							}

							catch
							{
								continue;
							}
						}

						break;

					case "s":
					case "size":
						try
						{
							SudokuPuzzleTest.Size = Int32.Parse(value);
							break;
						}

						catch
						{
							continue;
						}

					default:
						continue;
				}
			}
		}

		private int Generate(bool includeUpperLimit = false) => includeUpperLimit ? this.Random.Next(SudokuPuzzleTest.Size + 1) : this.Random.Next(SudokuPuzzleTest.Size);

		private void TestSetNumberFromZeroToNumber(out int row, out int column, out int number)
		{
			row = this.Generate();
			column = this.Generate();
			number = this.Generate(true);
			List<int> affectedPossibleList = new List<int>(this.Possible);
			affectedPossibleList.Remove(number);
			int[] affectedPossible = affectedPossibleList.ToArray();
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			this.Sudoku[row, column] = number;

			for (int i = 0; i < SudokuPuzzleTest.Size; i ++)
			{
				for (int j = 0; j < SudokuPuzzleTest.Size; j ++)
				{
					this.Sudoku.GetStartRowColumn(i, j, out int currentStartRow, out int currentStartColumn);

					if (i == row || j == column || currentStartRow == startRow && currentStartColumn == startColumn)
					{
						//Check against affectedPossible
						if (this.Sudoku[i, j] == 0)
						{
							Assert.IsTrue(SudokuCellTest.Equal(affectedPossible, this.Sudoku.GetPossible(i, j)));
						}

						else
						{
							Assert.IsNull(this.Sudoku.GetPossible(i, j));
						}
					}

					else
					{
						if (this.Sudoku[i, j] == 0)
						{
							Assert.IsTrue(SudokuCellTest.Equal(this.Possible, this.Sudoku.GetPossible(i, j)));
						}

						else
						{
							Assert.IsNull(this.Sudoku.GetPossible(i, j));
						}
					}
				}
			}
		}

		[TestMethod]
		public void TestChanged()
		{
			bool changed = false;
			this.Sudoku.Changed += new EventHandler<SudokuEventArgs>((sender, e) => changed = true);
			const int ROW = 0, COLUMN = 0;
			int value = this.Sudoku[ROW, COLUMN], newValue;

			do
			{
				newValue = this.Random.Next(this.Sudoku.Size);
			} while (value == newValue);

			this.Sudoku[ROW, COLUMN] = newValue;
			Assert.IsTrue(changed);
		}

		[TestMethod]
		public void TestSetNumberFromZeroToNumber()
		{
			this.TestSetNumberFromZeroToNumber(out int row, out int column, out int number);
		}

		[TestMethod]
		public void TestSetNumberFromNumberToZero()
		{
			this.TestSetNumberFromZeroToNumber(out int row, out int column, out int number);
			this.Sudoku[row, column] = 0;

			for (int i = 0; i < SudokuPuzzleTest.Size; i ++)
			{
				for (int j = 0; j < SudokuPuzzleTest.Size; j ++)
				{
					if (this.Sudoku[i, j] == 0)
					{
						Assert.IsTrue(SudokuCellTest.Equal(this.Possible, this.Sudoku.GetPossible(i, j)));
					}

					else
					{
						Assert.IsNull(this.Sudoku.GetPossible(i, j));
					}
				}
			}
		}

		[TestMethod]
		public void TestSetNumberFromNumberToNumber()
		{
			this.TestSetNumberFromZeroToNumber(out int row, out int column, out int oldNumber);
			int number;

			do
			{
				number = this.Generate(true);
			} while (number != oldNumber);

			List<int> affectedPossibleList = new List<int>(this.Possible);
			affectedPossibleList.Remove(number);
			int[] affectedPossible = affectedPossibleList.ToArray();
			this.Sudoku.GetStartRowColumn(row, column, out int startRow, out int startColumn);
			this.Sudoku[row, column] = number;

			for (int i = 0; i < SudokuPuzzleTest.Size; i ++)
			{
				for (int j = 0; j < SudokuPuzzleTest.Size; j ++)
				{
					this.Sudoku.GetStartRowColumn(i, j, out int currentStartRow, out int currentStartColumn);

					if (i == row || j == column || currentStartRow == startRow && currentStartColumn == startColumn)
					{
						//Check against affectedPossible
						if (this.Sudoku[i, j] == 0)
						{
							Assert.IsTrue(SudokuCellTest.Equal(affectedPossible, this.Sudoku.GetPossible(i, j)));
						}

						else
						{
							Assert.IsNull(this.Sudoku.GetPossible(i, j));
						}
					}

					else if (this.Sudoku[i, j] == 0)
					{
						Assert.IsTrue(SudokuCellTest.Equal(this.Possible, this.Sudoku.GetPossible(i, j)));
					}

					else
					{
						Assert.IsNull(this.Sudoku.GetPossible(i, j));
					}
				}
			}
		}

		[TestMethod]
		public void TestConstructor()
		{
			Assert.IsNotNull(this.Sudoku);
			Assert.AreEqual(this.Sudoku.Size, SudokuPuzzleTest.Size);
			Assert.AreEqual(this.Sudoku.Difficulty, SudokuPuzzleTest.Difficulty);

			for (int i = 0; i < SudokuPuzzleTest.Size; i ++)
			{
				for (int j = 0; j < SudokuPuzzleTest.Size; j ++)
				{
					Assert.IsTrue(SudokuCellTest.Equal(this.Possible, this.Sudoku.GetPossible(i, j)));
				}
			}
		}

		[TestMethod]
		public void TestEvents()
		{
			bool eventSignal = false;
			this.Sudoku.Changed += new EventHandler<SudokuEventArgs>((sender, e) => eventSignal = true);
			SudokuSolver.RecursiveSolve(this.Sudoku);
			Assert.IsTrue(eventSignal);
		}

		[TestMethod]
		public void TestProperties()
		{
			int blockSize = (int) Math.Sqrt(this.Sudoku.Size);
			Assert.AreEqual(blockSize, this.Sudoku.BlockSize);
			Assert.IsFalse(this.Sudoku.IsComplete);
			SudokuSolver.RecursiveSolve(this.Sudoku);
			Assert.IsTrue(this.Sudoku.IsComplete);
			Random random = new Random();
			int row = random.Next(SudokuPuzzle.MaximumSupportedSize), column = random.Next(SudokuPuzzle.MaximumSupportedSize);
			this.Sudoku[row, column] = 0;
			Assert.IsFalse(this.Sudoku.IsComplete);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => this.Sudoku[-1, -1]);
			Assert.ThrowsException<ArgumentException>(() => this.Sudoku[SudokuPuzzle.MaximumSupportedSize, SudokuPuzzle.MaximumSupportedSize]);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => this.Sudoku[0, 0] = -1);
			Assert.ThrowsException<ArgumentException>(() => this.Sudoku[0, 0] = SudokuPuzzle.MaximumSupportedSize + 1);
			SudokuGenerator.RemoveNumbers(this.Sudoku);
			const int NUMBER = 0;

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					if (this.Sudoku.CheckReadOnly(i, j))
					{
						Assert.ThrowsException<SudokuCellReadOnlyException>(() => this.Sudoku[i, j] = NUMBER);
					}

					else
					{
						try
						{
							this.Sudoku[i, j] = NUMBER;
						}

						catch (Exception ex)
						{
							Assert.Fail($"Threw exception: {ex}");
						}
					}
				}
			}
		}

		[TestMethod]
		public void TestCheck()
		{
			SudokuGenerator.AddNumbers(this.Sudoku);
			Assert.IsTrue(this.Sudoku.Check());
			SudokuGenerator.RemoveNumbers(this.Sudoku);
			Assert.IsFalse(this.Sudoku.Check());
			SudokuSolver.RecursiveSolve(this.Sudoku);
			Assert.IsTrue(this.Sudoku.Check());
			Random random = new Random();
			int row, column;

			do
			{
				row = random.Next(this.Sudoku.Size);
				column = random.Next(this.Sudoku.Size);
			} while (this.Sudoku.CheckReadOnly(row, column));

			int number = this.Sudoku[row, column];
			this.Sudoku[row, column] = 0;
			Assert.IsFalse(this.Sudoku.Check());
			this.Sudoku[row, column] = number;
			Assert.IsTrue(this.Sudoku.Check());
		}

		[TestMethod]
		public void TestClone()
		{
			SudokuPuzzle clone = (SudokuPuzzle) this.Sudoku.Clone();
			Assert.AreEqual(this.Sudoku.Size, clone.Size);
			Assert.AreEqual(this.Sudoku.Difficulty, clone.Difficulty);
			
			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					Assert.AreEqual(this.Sudoku[i, j], clone[i, j]);
				}
			}
		}

		[TestMethod]
		public void TestColumns()
		{
			SudokuSolver.RecursiveSolve(this.Sudoku);
			int row = 0, column = 0;

			foreach (IEnumerable<int> item in this.Sudoku.Columns)
			{
				foreach (int number in item)
				{
					if (column == this.Sudoku.Size)
					{
						Assert.Fail();
					}

					Assert.AreEqual(this.Sudoku[row, column], number);
					row++;

					if (row == this.Sudoku.Size)
					{
						row = 0;
						column ++;
					}
				}
			}

			Assert.AreEqual(row, 0);
			Assert.AreEqual(column, this.Sudoku.Size);
		}

		[TestMethod]
		public void TestContainsPossible()
		{
			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					if (this.Sudoku[i, j] != 0)
					{
						for (int k = 0; k < this.Sudoku.Size; k++)
						{
							Assert.IsFalse(this.Sudoku.ContainsPossible(i, j, k + 1));
						}
					}

					else
					{
						bool containsAny = false;
						Assert.IsFalse(this.Sudoku.ContainsPossible(i, j, 0));

						for (int k = 0; k < this.Sudoku.Size; k ++)
						{
							if (this.Sudoku.ContainsPossible(i, j, k))
							{
								containsAny = true;
								break;
							}
						}

						if (!containsAny)
						{
							Assert.Fail($"An empty cell has no possible values at row: {i + 1}, column: {j + 1}");
						}
					}
				}
			}
		}

		private void TestEnumerator(IEnumerable<IEnumerable<int>> enumerator)
		{
			int row = 0, column = 0;

			foreach (IEnumerable<int> item in enumerator)
			{
				foreach (int number in item)
				{
					if (row == this.Sudoku.Size)
					{
						Assert.Fail();
					}

					Assert.AreEqual(this.Sudoku[row, column], number);
					column ++;

					if (column == this.Sudoku.Size)
					{
						column = 0;
						row ++;
					}
				}
			}

			Assert.AreEqual(row, this.Sudoku.Size);
			Assert.AreEqual(column, 0);
		}

		[TestMethod]
		public void TestEquals()
		{
			SudokuDifficulty newDifficulty = SudokuPuzzleTest.Difficulty == SudokuDifficulty.None ? SudokuDifficulty.Easy : SudokuDifficulty.None;
			SudokuPuzzle puzzle = new SudokuPuzzle(this.Sudoku.Size, newDifficulty);
			SudokuGenerator.AddNumbers(puzzle);

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					this.Sudoku[i, j] = puzzle[i, j];
				}
			}

			Assert.AreEqual(this.Sudoku, puzzle);
			Assert.IsTrue(this.Sudoku.Equals(puzzle));
			Assert.IsTrue(this.Sudoku == puzzle);
			Assert.IsFalse(this.Sudoku != puzzle);
			Assert.IsFalse(this.Sudoku == null);
			Assert.IsFalse(null == this.Sudoku);
			SudokuPuzzle nullPuzzle = null;
			Assert.IsTrue(null == nullPuzzle);
			this.Sudoku[0, 0] = 0;
			Assert.AreNotEqual(this.Sudoku, puzzle);
		}

		[TestMethod]
		public void TestGetEnumerator() => this.TestEnumerator(this.Sudoku);

		[TestMethod]
		public void TestGetHashCode()
		{
			SudokuDifficulty newDifficulty = SudokuPuzzleTest.Difficulty == SudokuDifficulty.None ? SudokuDifficulty.Easy : SudokuDifficulty.None;
			SudokuPuzzle puzzle = new SudokuPuzzle(this.Sudoku.Size, newDifficulty);
			SudokuGenerator.AddNumbers(puzzle);

			for (int i = 0; i < this.Sudoku.Size; i++)
			{
				for (int j = 0; j < this.Sudoku.Size; j++)
				{
					this.Sudoku[i, j] = puzzle[i, j];
				}
			}

			int hashCode = puzzle.GetHashCode();
			Assert.AreEqual(this.Sudoku, puzzle);
			Assert.AreEqual(this.Sudoku.GetHashCode(), hashCode);
			this.Sudoku[0, 0] = 0;
			Assert.AreNotEqual(this.Sudoku, puzzle);
			Assert.AreNotEqual(this.Sudoku.GetHashCode(), hashCode);
		}

		[TestMethod]
		public void TestGetIncorrect()
		{
			SudokuGenerator.AddNumbers(this.Sudoku);
			Random random = new Random();

			for (int i = 0; i < SudokuPuzzle.MaximumSupportedSize * 2; i ++)
			{
				int row = random.Next(this.Sudoku.Size), column = random.Next(this.Sudoku.Size);
				int number;

				do
				{
					number = random.Next(0, this.Sudoku.Size + 1);
				} while (this.Sudoku[row, column] == number);
			}

			IEnumerable<Tuple<int, int>> incorrect = this.Sudoku.GetIncorrect();

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					if (!this.Sudoku.IsCorrect(i, j))
					{
						Assert.IsTrue(incorrect.Contains(new Tuple<int, int>(i, j)));
					}
				}
			}
		}

		[TestMethod]
		public void TestOperators()
		{
			SudokuGenerator.AddNumbers(this.Sudoku);
			SudokuPuzzle clone = (SudokuPuzzle) this.Sudoku.Clone();
			Assert.AreEqual(this.Sudoku, clone);
			Assert.IsTrue(this.Sudoku.Equals(clone));
			Assert.IsTrue(this.Sudoku == clone);
			int row = 0, column = 0;
			clone[row, column] = 0;
			Assert.AreNotEqual(this.Sudoku, clone);
			Assert.IsFalse(this.Sudoku.Equals(clone));
			Assert.IsFalse(this.Sudoku == clone);
			Assert.IsTrue(this.Sudoku != clone);
		}

		[TestMethod]
		public void TestRows() => this.TestEnumerator(this.Sudoku.Rows);
	}
}
