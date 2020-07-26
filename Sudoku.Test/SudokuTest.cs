using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.Test
{
	[TestClass]
	public class SudokuTest
	{
		public const SudokuDifficulty DefaultDifficulty = SudokuDifficulty.Easy;
		public const int DefaultSize = 9;
		private const char SeparatorChar = '=';

		public static SudokuDifficulty Difficulty { get; private set; }
		public static int Size { get; private set; }

		private int[] Possible { get; }
		private Random Random { get; }
		private Sudoku Sudoku { get; set; }

		static SudokuTest()
		{
			try
			{
				if (File.Exists("vars.txt"))
				{
					SudokuTest.ParseFile("vars.txt");
				}

				else if (File.Exists("Variables.txt"))
				{
					SudokuTest.ParseFile("Variables.txt");
				}

				else
				{
					throw new Exception();
				}
			}

			catch
			{
				SudokuTest.Difficulty = SudokuTest.DefaultDifficulty;
				SudokuTest.Size = SudokuTest.DefaultSize;
			}

			Assert.AreNotEqual(SudokuTest.Difficulty, SudokuDifficulty.None, "Difficulty must not be None");
			Assert.IsTrue(Sudoku.VerifySize(SudokuTest.Size), "Size must be a positive, square integer");
		}

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

		private static void ParseFile(String name)
		{
			String[] lines = File.ReadAllLines(name);

			foreach (String line in lines)
			{
				String formattedLine = line.ToLower();
				formattedLine = Regex.Replace(formattedLine, @"\s+", "");

				if (formattedLine.Count(c => c == SudokuTest.SeparatorChar) != 1)
				{
					continue;
				}

				int index = formattedLine.IndexOf(SudokuTest.SeparatorChar);
				String key = formattedLine.Substring(0, index);
				String value = formattedLine.Substring(index + 1);

				switch (key)
				{
					case "d":
					case "diff":
					case "difficulty":
						try
						{
							SudokuTest.Difficulty = (SudokuDifficulty) Enum.Parse(typeof (SudokuDifficulty), value, true);
						}

						catch
						{
							try
							{
								uint parsedValue = UInt32.Parse(value);
								SudokuTest.Difficulty = (SudokuDifficulty) parsedValue;
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
							SudokuTest.Size = Int32.Parse(value);
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

		private int Generate(bool includeUpperLimit = false) => includeUpperLimit ? this.Random.Next(SudokuTest.Size + 1) : this.Random.Next(SudokuTest.Size);

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

			for (int i = 0; i < SudokuTest.Size; i ++)
			{
				for (int j = 0; j < SudokuTest.Size; j ++)
				{
					this.Sudoku.GetStartRowColumn(i, j, out int currentStartRow, out int currentStartColumn);

					if (i == row || j == column || currentStartRow == startRow && currentStartColumn == startColumn)
					{
						//Check against affectedPossible
						if (this.Sudoku[i, j] == 0)
						{
							Assert.IsTrue(SudokuTest.ArraysEqual<int>(affectedPossible, this.Sudoku.GetPossible(i, j)));
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
							Assert.IsTrue(SudokuTest.ArraysEqual<int>(this.Possible, this.Sudoku.GetPossible(i, j)));
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
		public void TestSetNumberFromZeroToNumber()
		{
			this.TestSetNumberFromZeroToNumber(out int row, out int column, out int number);
		}

		[TestMethod]
		public void TestSetNumberFromNumberToZero()
		{
			this.TestSetNumberFromZeroToNumber(out int row, out int column, out int number);
			this.Sudoku[row, column] = 0;

			for (int i = 0; i < SudokuTest.Size; i ++)
			{
				for (int j = 0; j < SudokuTest.Size; j ++)
				{
					if (this.Sudoku[i, j] == 0)
					{
						Assert.IsTrue(SudokuTest.ArraysEqual<int>(this.Possible, this.Sudoku.GetPossible(i, j)));
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

			for (int i = 0; i < SudokuTest.Size; i ++)
			{
				for (int j = 0; j < SudokuTest.Size; j ++)
				{
					this.Sudoku.GetStartRowColumn(i, j, out int currentStartRow, out int currentStartColumn);

					if (i == row || j == column || currentStartRow == startRow && currentStartColumn == startColumn)
					{
						//Check against affectedPossible
						if (this.Sudoku[i, j] == 0)
						{
							Assert.IsTrue(SudokuTest.ArraysEqual<int>(affectedPossible, this.Sudoku.GetPossible(i, j)));
						}

						else
						{
							Assert.IsNull(this.Sudoku.GetPossible(i, j));
						}
					}

					else if (this.Sudoku[i, j] == 0)
					{
						Assert.IsTrue(SudokuTest.ArraysEqual<int>(this.Possible, this.Sudoku.GetPossible(i, j)));
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

		[TestMethod]
		public void TestOperators()
		{
			SudokuGenerator.AddNumbers(this.Sudoku);
			Sudoku clone = (Sudoku) this.Sudoku.Clone();
			Assert.AreEqual(this.Sudoku, clone);
			Assert.IsTrue(this.Sudoku.Equals(clone));
			Assert.IsTrue(this.Sudoku == clone);
			//int row = this.Generate(), column = this.Generate();
			int row = 0, column = 0;
			clone[row, column] = 0;
			Assert.AreNotEqual(this.Sudoku, clone);
			Assert.IsFalse(this.Sudoku.Equals(clone));
			Assert.IsFalse(this.Sudoku == clone);
			Assert.IsTrue(this.Sudoku != clone);
		}
	}
}
