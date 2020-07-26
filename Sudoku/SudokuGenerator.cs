﻿using System;
using System.Collections.Generic;

namespace Sudoku
{
	public static class SudokuGenerator
	{
		private static readonly Random Random = new Random();

		public static void AddNumbers(Sudoku sudoku)
		{
			if (sudoku is null)
			{
				throw new NotImplementedException(nameof (sudoku));
			}

			sudoku.DisablePossible = true;
			SudokuSolver.RecursiveSolve(sudoku);
			sudoku.DisablePossible = false;
			sudoku.ResetPossible();
		}

		public static Sudoku AddNumbers(int size, SudokuDifficulty difficulty)
		{
			if (difficulty == SudokuDifficulty.None)
			{
				throw new ArgumentException(nameof(difficulty));
			}

			Sudoku sudoku = new Sudoku(size, difficulty);
			SudokuGenerator.AddNumbers(sudoku);
			return sudoku;
		}

		public static Sudoku Generate(int size, SudokuDifficulty difficulty)
		{
			Sudoku sudoku = SudokuGenerator.AddNumbers(size, difficulty);
			//SudokuGenerator.RemoveNumbers(sudoku);
			return sudoku;
		}

		public static void RemoveNumbers(Sudoku sudoku)
		{
			if (sudoku is null)
			{
				throw new NotImplementedException(nameof(sudoku));
			}

			throw new NotImplementedException();
		}

		internal static void ShuffleNumbers<T>(List<T> array)
		{
			for (int i = 0; i < array.Count - 1; i++)
			{
				int j = SudokuGenerator.Random.Next(i, array.Count);
				T temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
		}
	}
}
