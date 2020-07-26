using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sudoku.UI.Test
{
	[TestClass]
	public class ProgramTest
	{
		[TestMethod]
		public void TestCompareArguments()
		{
			const String BAD_FILENAME = "";
			const String GOOD_FILENAME = "Sudoku.txt";

			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-f", BAD_FILENAME, "-o" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-f", BAD_FILENAME, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-f", GOOD_FILENAME, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-f", "-o" })); //Files can start with hyphens
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "COMPARE", "-f", GOOD_FILENAME }));

			if (File.Exists("-MISSING"))
			{
				File.Delete("-MISSING");
			}

			Assert.AreEqual(Program.FileNotFoundError, Program.Main(new String[] { "COMPARE", "-f", "-MISSING", "-o", "-MISSING" })); //Files can start with hyphens
			Assert.AreEqual(0, Program.Main(new String[] { "COMPARE", "-f", GOOD_FILENAME, "-o", GOOD_FILENAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "C", "-f", GOOD_FILENAME, "-o", GOOD_FILENAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "COMP", "-f", GOOD_FILENAME, "-o", GOOD_FILENAME }));
		}

		[TestMethod]
		public void TestGenerateArguments()
		{
			const int NEGATIVE_SIZE = -1;
			const int ZERO_SIZE = 0;
			const int BAD_POSITIVE_SIZE = 2;
			const int GOOD_POSITIVE_SIZE = 9;
			const SudokuDifficulty BAD_DIFF = SudokuDifficulty.None;
			const SudokuDifficulty GOOD_DIFF = SudokuDifficulty.Easy;
			const String BAD_DIFF_STRING = "NONE";
			const String BAD_DIFF_FORMAT = "E";
			const String GOOD_DIFF_STRING = "EaSy";
			const String BAD_FILENAME = "";
			const String GOOD_FILENAME = "Sudoku.txt";
			const String GOOD_OUTNAME = "SudokuGenerate.txt";

			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", "-d" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", "-d", "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString() }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", NEGATIVE_SIZE.ToString(), "-d", BAD_DIFF.ToString(), "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", ZERO_SIZE.ToString(), "-d", BAD_DIFF.ToString(), "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", BAD_POSITIVE_SIZE.ToString(), "-d", BAD_DIFF.ToString(), "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", BAD_DIFF.ToString(), "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", BAD_DIFF_STRING, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", BAD_DIFF_FORMAT, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF_STRING, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF_STRING, "-o", GOOD_FILENAME, "-XXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF_STRING, "-o", GOOD_FILENAME, "XXX" }));
			Assert.AreEqual(0, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF_STRING, "-o", GOOD_OUTNAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF_STRING }));
			Assert.AreEqual(0, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF.ToString(), "-o", GOOD_OUTNAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "GENERATE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF.ToString() }));
			Assert.AreEqual(0, Program.Main(new String[] { "G", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF.ToString() }));
			Assert.AreEqual(0, Program.Main(new String[] { "GEN", "-s", GOOD_POSITIVE_SIZE.ToString(), "-d", GOOD_DIFF.ToString() }));
		}

		[TestMethod]
		public void TestInteractiveArguments()
		{
			const int NEGATIVE_SIZE = -1;
			const int ZERO_SIZE = 0;
			const int BAD_POSITIVE_SIZE = 2;
			const int GOOD_POSITIVE_SIZE = 9;
			const String BAD_FILENAME = "";

			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", NEGATIVE_SIZE.ToString() }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", ZERO_SIZE.ToString() }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", BAD_POSITIVE_SIZE.ToString() }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "INTERACTIVE", "-s", GOOD_POSITIVE_SIZE.ToString(), "-f", BAD_FILENAME }));
		}

		[TestMethod]
		public void TestPrintArguments()
		{
			const String BAD_FILENAME = "";
			const String GOOD_FILENAME = "Sudoku.txt";

			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "PRINT" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "PRINT", "XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "PRINT", "-XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "PRINT", "-f" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "PRINT", "-f", BAD_FILENAME }));

			if (File.Exists("-MISSING"))
			{
				File.Delete("-MISSING");
			}

			Assert.AreEqual(Program.FileNotFoundError, Program.Main(new String[] { "PRINT", "-f", "-MISSING" })); //Files can start with hyphens
			Assert.AreEqual(0, Program.Main(new String[] { "PRINT", "-f", GOOD_FILENAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "P", "-f", GOOD_FILENAME }));
		}

		[TestMethod]
		public void TestSolveArguments()
		{
			const String BAD_FILENAME = "";
			const String GOOD_FILENAME = "Sudoku.txt";
			const String GOOD_OUTNAME = "SudokuSolve.txt";

			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE", "XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE", "-XXXXXX" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE", "-f", BAD_FILENAME, "-o" }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE", "-f", BAD_FILENAME, "-o", BAD_FILENAME }));
			Assert.AreEqual(Program.UsageMessageWarning, Program.Main(new String[] { "SOLVE", "-f", GOOD_FILENAME, "-o", BAD_FILENAME }));

			if (File.Exists("-MISSING"))
			{
				File.Delete("-MISSING");
			}

			Assert.AreEqual(Program.FileNotFoundError, Program.Main(new String[] { "SOLVE", "-f", "-MISSING" })); //Files can start with hyphens
			Assert.AreEqual(0, Program.Main(new String[] { "SOLVE", "-f", GOOD_FILENAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "SOLVE", "-f", GOOD_FILENAME, "-o", GOOD_OUTNAME }));
			Assert.AreEqual(0, Program.Main(new String[] { "S", "-f", GOOD_FILENAME, "-o", GOOD_OUTNAME }));
		}
	}
}
