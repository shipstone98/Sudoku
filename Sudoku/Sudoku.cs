using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
	/// <summary>
	/// Represents a sudoku puzzle.
	/// </summary>
	public class Sudoku: ICloneable, IEquatable<Sudoku>
	{
		public Object Clone() => throw new NotImplementedException();

		public bool Equals(Sudoku sudoku) => throw new NotImplementedException();
	}
}
