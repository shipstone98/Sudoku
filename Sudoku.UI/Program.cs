﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Sudoku.UI.Test")]
namespace Sudoku.UI
{
	internal static class Program
	{
		private const SudokuDifficulty DefaultDifficulty = SudokuDifficulty.Easy;
		private const int DefaultSize = 9;

		internal const int IOError = -4;
		internal const int FileOutputError = -3;
		internal const int FileFormatIncorrectError = -2;
		internal const int FileNotFoundError = -1;

		internal const int UsageMessageWarning = 1;

		private const int CompareCommand = 0;
		private const int GenerateCommand = 1;
		private const int InteractiveCommand = 4;
		private const int PrintCommand = 2;
		private const int SolveCommand = 3;
		private const int EnumerateCommand = 5;

		private const String IOErrorMessage = "ERROR: couldn't opening the I/O streams";
		private const String FileOutputErrorMessage = "ERROR: couldn't write to file";
		private const String FileFormatIncorrectErrorMessage = "ERROR: file format not in correct order";
		private const String FileNotFoundErrorMessage = "ERROR: file not found or couldn't be opened";
		private const String UsageMessage = "Usage: sudoku command [options]\n\nCommmands\n\t- COMPARE: Compare two puzzles, must specify both filenames with -f and -o\n\t- ENUMERATE: Checks if a puzzle is solvable, and whether it has multiple valid solutions, must specify filename with -f.\n\t- GENERATE: Generate a new sudoku puzzle, must specify size with -s and difficulty with -d. Size must be positive square integer and difficulty must be easy, medium or hard. Can optionally specify an output filename with -o\n\t- INTERACTIVE: Launch an interactive window to play sudoku. The default size is 9 but can optionally be set with -s. If no size is set, instead optionally specify a puzzle with -f.\n\t- PRINT: Print an existing sudoku puzzle to the screen, must specify filename with -f\n\t- SOLVE: Solves an existing sudoku puzzle, must specify filename with -f. Can optionally specify an output filename with -o\n\nAll options are NOT case sensitive; however, some operating systems may treat entered filenames as case sensitive (Windows doesn't but Mac OS and Linux generally do)";

		private static int Compare(String file1, String file2)
		{
			String text1, text2;

			try
			{
				text1 = File.ReadAllText(file1);
				text2 = File.ReadAllText(file2);
			}

			catch
			{
				Console.WriteLine(Program.FileNotFoundErrorMessage);
				return Program.FileNotFoundError;
			}

			SudokuPuzzle a, b;
			
			try
			{
				a = SudokuPuzzle.Parse(text1);
				b = SudokuPuzzle.Parse(text2);
			}

			catch (FormatException)
			{
				Console.WriteLine(Program.FileFormatIncorrectErrorMessage);
				return Program.FileFormatIncorrectError;
			}

			Console.WriteLine("Sudoku puzzle A:");
			Console.WriteLine(a);
			Console.WriteLine();
			Console.WriteLine("Sudoku puzzle B:");
			Console.WriteLine(b);
			Console.WriteLine();
			Console.WriteLine(a == b ? "The two puzzles are equal." : "The two puzzles are NOT equal.");
			return 0;
		}

		private static int Enumerate(String filename)
		{
			String text;

			if (!File.Exists(filename))
			{
				Console.WriteLine(Program.FileNotFoundErrorMessage);
				return Program.FileNotFoundError;
			}

			try
			{
				text = File.ReadAllText(filename);
			}

			catch
			{
				Console.WriteLine(Program.FileFormatIncorrectErrorMessage);
				return Program.FileFormatIncorrectError;
			}

			SudokuPuzzle sudoku;

			try
			{
				sudoku = SudokuPuzzle.Parse(text);
			}

			catch (FormatException)
			{
				Console.WriteLine(Program.FileFormatIncorrectErrorMessage);
				return Program.FileFormatIncorrectError;
			}

			bool solvable = SudokuSolver.CheckSolvable(sudoku, out bool multipleSolutions);

			if (solvable)
			{
				Console.WriteLine("The specified puzzle is solvable.");

				if (multipleSolutions)
				{
					Console.WriteLine("The specified puzzle has multiple possible solutions.");
				}

				else
				{
					Console.WriteLine("The specified puzzle has a single valid solution.");
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					SudokuSolver.RecursiveSolve(sudoku);
					stopwatch.Stop();
					Console.WriteLine(sudoku);
					Console.WriteLine($"Puzzle solved recursively in {stopwatch.ElapsedMilliseconds}ms");
				}
			}

			else
			{
				Console.WriteLine("The specified puzzle is NOT solvable.");
			}

			return 0;
		}
		
		private static int Generate(int size, SudokuDifficulty difficulty, String outfile)
		{
			SudokuPuzzle sudoku = SudokuGenerator.Generate(size, difficulty);

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

		private static int Interactive(int size, String filename)
		{
			if (Console.IsInputRedirected || Console.IsOutputRedirected)
			{
				Console.WriteLine(Program.IOErrorMessage);
				return Program.IOError;
			}

			Window window = filename is null ? new Window(size) : new Window(filename);
			window.Init();
			return 0;
		}

		internal static int Main(String[] args)
		{
			if (!Console.IsInputRedirected)
			{
				Console.Title = "Sudoku";
			}

			if (args.Length == 0)
			{
				if (Console.IsInputRedirected || Console.IsOutputRedirected)
				{
					Console.WriteLine(Program.IOErrorMessage);
					return Program.IOError;
				}

				Window window = new Window(SudokuGenerator.Generate(Program.DefaultSize, Program.DefaultDifficulty));
				window.Init();
				return 0;
			}

			if (!Program.ParseArguments(args, out int command, out int size, out SudokuDifficulty difficulty, out String filename, out String outfile))
			{
				Console.WriteLine(Program.UsageMessage);
				return Program.UsageMessageWarning;
			}

			switch (command)
			{
				case Program.CompareCommand:
					return Program.Compare(filename, outfile);
				case Program.EnumerateCommand:
					return Program.Enumerate(filename);
				case Program.GenerateCommand:
					return Program.Generate(size, difficulty, outfile);
				case Program.InteractiveCommand:
					return Program.Interactive(size, filename);
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
				Console.WriteLine(SudokuPuzzle.Parse(text));
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
					case "c":
					case "comp":
					case "compare":
						command = Program.CompareCommand;
						break;
					case "e":
					case "enum":
					case "enumerate":
						command = Program.EnumerateCommand;
						break;
					case "g":
					case "gen":
					case "generate":
						command = Program.GenerateCommand;
						break;
					case "i":
					case "int":
					case "inter":
					case "interactive":
						command = Program.InteractiveCommand;
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
								if (command == Program.GenerateCommand || !(filename is null) || String.IsNullOrWhiteSpace(param) || command == Program.InteractiveCommand && size != 0)
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
								if (!(command == Program.GenerateCommand || command == Program.InteractiveCommand) || size != 0 || command == Program.InteractiveCommand && !(filename is null))
								{
									return false;
								}

								size = Int32.Parse(param);

								if (!SudokuPuzzle.VerifySize(size))
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
				case Program.CompareCommand:
					if (String.IsNullOrWhiteSpace(filename) || String.IsNullOrWhiteSpace(outfile))
					{
						return false;
					}

					break;

				case Program.EnumerateCommand:
					if (filename is null)
					{
						return false;
					}

					break;

				case Program.GenerateCommand:
					if (!SudokuPuzzle.VerifySize(size) || difficulty == SudokuDifficulty.None)
					{
						return false;
					}

					break;

				case Program.InteractiveCommand:
					if (filename is null && !SudokuPuzzle.VerifySize(size) || !(filename is null || size == 0))
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

			SudokuPuzzle sudoku;

			try
			{
				sudoku = SudokuPuzzle.Parse(text);
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
			const int PADDING = 16;
			const String SEPARATOR = "\t";
			sb.AppendLine(String.Join(SEPARATOR, '#', "Row(s)".PadRight(PADDING), "Column(s)".PadRight(PADDING), "Number(s)".PadRight(PADDING), "Pattern".PadRight(PADDING), "Possible".PadRight(PADDING)));

			foreach (SudokuMove move in moves)
			{
				sb.AppendLine(String.Join('\t', ++ count, move));
			}

			sb.AppendLine($"Solved puzzle using {count} moves in {time}ms");

			/*if (!sudoku.IsComplete)
			{
				stopwatch.Restart();
				SudokuSolver.RecursiveSolve(sudoku);
				stopwatch.Stop();
				double newTime = stopwatch.ElapsedMilliseconds;
				sb.AppendLine();
				sb.AppendLine(sudoku.ToString());
				sb.AppendLine();
				sb.AppendLine($"Solved remainder of puzzle using recursion in {newTime}ms");
				sb.AppendLine($"Total time for solving is {time + newTime} ms");
			}*/

			sb.AppendLine();
			sb.AppendLine("Solved sudoku:");
			sb.AppendLine(sudoku.ToString());
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
