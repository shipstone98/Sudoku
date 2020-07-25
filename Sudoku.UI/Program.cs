using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Sudoku.UI.Test")]
namespace Sudoku.UI
{
	public static class Program
	{
		internal const int FileOutputError = -3;
		internal const int FileFormatIncorrectError = -2;
		internal const int FileNotFoundError = -1;

		internal const int UsageMessageWarning = 1;

		private const int GenerateCommand = 1;
		private const int PrintCommand = 2;
		private const int SolveCommand = 3;

		private const String FileOutputErrorMessage = "ERROR: couldn't write to file";
		private const String FileFormatIncorrectErrorMessage = "ERROR: file format not in correct order";
		private const String FileNotFoundErrorMessage = "ERROR: file not found or couldn't be opened";
		private const String UsageMessage = "Usage: sudoku command [options]\n\nOptions\n\t- GENERATE: Generate a new sudoku puzzle, must specify size with -s and difficulty with -d. Size must be positive square integer and difficulty must be easy, medium or hard. Can optionally specify an output filename with -o\n\t- PRINT: Print an existing sudoku puzzle to the screen, must specify filename with -f\n\t-SOLVE: Solves an existing sudoku puzzle, must specify filename with -f. Can optionally specify an output filename with -o\n\nAll options are NOT case sensitive; however, some operating systems may treat entered filenames as case sensitive (Windows doesn't but Mac OS and Linux generally do)";

		private static int Generate(int size, SudokuDifficulty difficulty, String outfile)
		{
			Sudoku sudoku = SudokuGenerator.Generate(size, difficulty);

			if (outfile is null)
			{
				Console.WriteLine(sudoku);
			}

			else
			{
				if (Program.WriteToFile(sudoku.ToString(), outfile))
				{
					Console.WriteLine($"Generated sudoku written to {outfile}");
				}

				else
				{
					Console.WriteLine(Program.FileOutputErrorMessage);
					return Program.FileOutputError;
				}
			}

			return 0;
		}

		public static int Main(String[] args)
		{
			if (!Program.ParseArguments(args, out int command, out int size, out SudokuDifficulty difficulty, out String filename, out String outfile))
			{
				Console.WriteLine(Program.UsageMessage);
				return Program.UsageMessageWarning;
			}

			switch (command)
			{
				case Program.GenerateCommand:
					return Program.Generate(size, difficulty, outfile);
				case Program.PrintCommand:
					return Program.Print(filename);
				case Program.SolveCommand:
					return Program.Solve(filename, outfile);
				default:
					Console.WriteLine(Program.UsageMessage);
					return Program.UsageMessageWarning;
			}
		}

		private static int Print(String filename)
		{
			String text;

			try
			{
				text = File.ReadAllText(filename);
			}

			catch
			{
				Console.WriteLine(Program.FileNotFoundErrorMessage);
				return Program.FileNotFoundError;
			}

			try
			{
				Console.WriteLine(Sudoku.Parse(text));
				return 0;
			}

			catch (FormatException)
			{
				Console.WriteLine(Program.FileFormatIncorrectErrorMessage);
				return Program.FileFormatIncorrectError;
			}
		}

		private static bool ParseArguments(String[] args, out int command, out int size, out SudokuDifficulty difficulty, out String filename, out String outfile)
		{
			command = size = 0;
			filename = outfile = null;
			difficulty = SudokuDifficulty.None;

			try
			{
				switch (args[0].ToLower())
				{
					case "g":
					case "gen":
					case "generate":
						command = Program.GenerateCommand;
						break;

					case "p":
					case "print":
						command = Program.PrintCommand;
						break;

					case "s":
					case "solve":
						command = Program.SolveCommand;
						break;

					default:
						return false;
				}

				for (int i = 1; i < args.Length; i ++)
				{
					if (String.IsNullOrWhiteSpace(args[i]))
					{
						continue;
					}

					String arg = Regex.Replace(args[i].ToLower(), @"\s+", "");

					if (arg[0] == '-')
					{
						if (arg.Length > 2)
						{
							return false;
						}

						//String param = Regex.Replace(args[i + 1].ToLower(), @"\s+", "");
						String param = args[i + 1];

						switch (arg[1])
						{
							case 'd':
								if (command != Program.GenerateCommand || difficulty != SudokuDifficulty.None)
								{
									return false;
								}

								try
								{
									difficulty = (SudokuDifficulty) Enum.Parse(typeof (SudokuDifficulty), param, true);
								}

								catch
								{
									difficulty = (SudokuDifficulty) Int32.Parse(param);
								}

								if (difficulty == SudokuDifficulty.None)
								{
									return false;
								}

								break;

							case 'f':
								if (command == Program.GenerateCommand || !(filename is null) || String.IsNullOrWhiteSpace(param))
								{
									return false;
								}

								filename = param;
								break;

							case 'o':
								if (command == Program.PrintCommand || !(outfile is null) || String.IsNullOrWhiteSpace(param))
								{
									return false;
								}

								outfile = param;
								break;

							case 's':
								if (command != Program.GenerateCommand || size != 0)
								{
									return false;
								}

								size = Int32.Parse(param);

								if (!Sudoku.VerifySize(size))
								{
									return false;
								}

								break;

							default:
								return false;
						}

						i ++;
					}

					else
					{
						return false;
					}
				}
			}

			catch
			{
				return false;
			}

			switch (command)
			{
				case Program.GenerateCommand:
					if (!Sudoku.VerifySize(size) || difficulty == SudokuDifficulty.None)
					{
						return false;
					}

					break;

				case Program.PrintCommand:
					if (String.IsNullOrWhiteSpace(filename))
					{
						return false;
					}

					break;

				case Program.SolveCommand:
					if (String.IsNullOrWhiteSpace(filename) || !(outfile is null) && String.IsNullOrWhiteSpace(outfile))
					{
						return false;
					}

					break;

				default:
					return false;
			}

			return true;
		}

		private static int Solve(String filename, String outfile)
		{
			String text;

			try
			{
				text = File.ReadAllText(filename);
			}

			catch
			{
				Console.WriteLine(Program.FileNotFoundErrorMessage);
				return Program.FileNotFoundError;
			}

			Sudoku sudoku;

			try
			{
				sudoku = Sudoku.Parse(text);
			}

			catch (FormatException)
			{
				Console.WriteLine(Program.FileFormatIncorrectErrorMessage);
				return Program.FileFormatIncorrectError;
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Sudoku from file, unsolved:");
			sb.AppendLine(sudoku.ToString());
			sb.AppendLine();
			Stopwatch stopwatch = new Stopwatch();
			SudokuSolver solver = new SudokuSolver(sudoku);
			stopwatch.Start();
			IEnumerable<SudokuMove> moves = solver.Solve();
			stopwatch.Stop();
			double time = stopwatch.ElapsedMilliseconds;
			int count = 0;
			sb.AppendLine("Solved sudoku:");
			sb.AppendLine(sudoku.ToString());
			sb.AppendLine();
			sb.AppendLine(String.Join('\t', '#', "Row", "Column", "Number", "Pattern", "Possible"));

			foreach (SudokuMove move in moves)
			{
				sb.AppendLine(String.Join('\t', ++ count, move));
			}

			sb.AppendLine($"Solved puzzle using {count} moves in {time}ms");
			String output = sb.ToString();

			if (outfile is null)
			{
				Console.WriteLine(output);
			}

			else
			{
				if (Program.WriteToFile(output, outfile))
				{
					Console.WriteLine($"Solved puzzle using {count} moves in {time}ms");
					Console.WriteLine($"Moves used to solve sudoku written to {outfile}");
				}

				else
				{
					Console.WriteLine(Program.FileOutputErrorMessage);
					return Program.FileOutputError;
				}
			}

			return 0;
		}

		private static bool WriteToFile(String s, String filename)
		{
			try
			{
				File.WriteAllText(filename, s);
				return true;
			}

			catch
			{
				return false;
			}
		}
	}
}
