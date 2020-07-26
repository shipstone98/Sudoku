using System;
using System.IO;

namespace Sudoku.UI
{
	internal class Window
	{
		private const int CursorXStart = 2;
		private const int CursorYStart = 2;
		private const int CursorXEnd = 22;
		private const int CursorYEnd = 12;

		private int CursorX { get; set; }
		private int CursorY { get; set; }
		internal int Size { get; }
		private Sudoku Sudoku { get; }
		private int UpperLimit { get; }
		private int X { get; set; }
		private int Y { get; set; }

		private Window()
		{
			this.CursorX = Window.CursorXStart;
			this.CursorY = Window.CursorYStart;
			this.Y = this.X = 0;
		}

		internal Window(int size) : this()
		{
			this.Size = size;
			this.Sudoku = new Sudoku(size, SudokuDifficulty.None);
			this.UpperLimit = this.Size - 1;
		}

		internal Window(String filename) : this()
		{
			String text = File.ReadAllText(filename);
			this.Sudoku = Sudoku.Parse(text);
			this.Size = this.Sudoku.Size;
			this.UpperLimit = this.Size - 1;
		}

		private bool HandleKeyPress(ConsoleKeyInfo cki)
		{
			if (cki.Key >= ConsoleKey.D0 && cki.Key <= ConsoleKey.D9 || cki.Key >= ConsoleKey.NumPad0 && cki.Key <= ConsoleKey.NumPad9)
			{
				this.EnterNumber(cki.KeyChar - '0');
			}

			else
			{
				switch (cki.Key)
				{
					case ConsoleKey.UpArrow:
					case ConsoleKey.W:
						if (this.Y == 0)
						{
							return false;
						}
							
						this.Y --;
						this.CursorY -= this.Y % this.Sudoku.BlockSize == 2 ? 2 : 1;
						break;

					case ConsoleKey.DownArrow:
					case ConsoleKey.S:
						if (this.Y == this.UpperLimit)
						{
							return false;
						}
							
						this.Y ++;
						this.CursorY += this.Y % this.Sudoku.BlockSize == 0 ? 2 : 1;
						break;

					case ConsoleKey.LeftArrow:
					case ConsoleKey.A:
						if (this.X == 0)
						{
							return false;
						}
							
						this.X --;
						this.CursorX -= this.X % this.Sudoku.BlockSize == 2 ? 4 : 2;
						break;

					case ConsoleKey.RightArrow:
					case ConsoleKey.D:
						if (this.X == this.UpperLimit)
						{
							return false;
						}
							
						this.X ++;
						this.CursorX += this.X % this.Sudoku.BlockSize == 0 ? 4 : 2;
						break;

					case ConsoleKey.Home:
					case ConsoleKey.Q:
						if (cki.Modifiers == ConsoleModifiers.Control)
						{
							this.Y = 0;
							this.CursorY = Window.CursorYStart;
						}

						this.X = 0;
						this.CursorX = Window.CursorXStart;
						break;

					case ConsoleKey.End:
					case ConsoleKey.E:
						if (cki.Modifiers == ConsoleModifiers.Control)
						{
							this.Y = this.UpperLimit;
							this.CursorY = Window.CursorYEnd;
						}

						this.X = this.UpperLimit;
						this.CursorX = Window.CursorXEnd;
						break;

					case ConsoleKey.PageUp:
						if (cki.Modifiers == ConsoleModifiers.Control)
						{
							this.X = 0;
							this.CursorX = Window.CursorXStart;
						}

						this.Y = 0;
						this.CursorY = Window.CursorYStart;
						break;

					case ConsoleKey.PageDown:
						if (cki.Modifiers == ConsoleModifiers.Control)
						{
							this.X = this.UpperLimit;
							this.CursorX = Window.CursorXEnd;
						}

						this.Y = this.UpperLimit;
						this.CursorY = Window.CursorYEnd;
						break;

					case ConsoleKey.Backspace:
						this.EnterNumber(0);
						break;

					case ConsoleKey.Spacebar:
						SudokuSolver.RecursiveSolve(this.Sudoku);
						break;
				}
			}

			return true;
		}

		internal void Init()
		{
			this.UpdateScreen();

			while (true)
			{
				ConsoleKeyInfo cki = Console.ReadKey(true);

				if (cki.Key == ConsoleKey.Escape)
				{
					return;
				}

				if (this.HandleKeyPress(cki))
				{
					this.UpdateScreen();
				}
			}
		}

		private void EnterNumber(int number) => this.Sudoku[this.Y, this.X] = number;

		private void UpdateScreen()
		{
			Console.Clear();
			Console.WriteLine($"Row: {this.Y + 1}\tColumn: {this.X + 1}");
			Console.WriteLine(this.Sudoku);
			Console.WriteLine("Press ESC to quit...");

			if (this.Sudoku[this.Y, this.X] == 0)
			{
				int[] possible = this.Sudoku.GetPossible(this.Y, this.X);
				Array.Sort(possible);
				Console.WriteLine($"Possible values: {String.Join(",", possible)}");
			}

			Console.CursorLeft = this.CursorX;
			Console.CursorTop = this.CursorY;
		}
	}
}
