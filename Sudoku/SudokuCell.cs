using System;
using System.Collections.Generic;

namespace Sudoku
{
	internal class SudokuCell: ICloneable, IEquatable<SudokuCell>
	{
		private readonly List<int> _Possible;

		internal bool IsCorrect => this.Number == this.Solution && this.Number != 0;
		internal bool IsReadOnly { get; set; }
		internal int Number { get; set; }
		internal int[] Possible => this._Possible.ToArray();
		internal int Solution { get; set; }

		internal SudokuCell()
		{
			this.Number = this.Solution = 0;
			this._Possible = new List<int>();
			this.IsReadOnly = false;
		}

		internal void AddPossible(int number)
		{
			if (!this._Possible.Contains(number))
			{
				this._Possible.Add(number);
			}
		}

		public Object Clone()
		{
			SudokuCell cell = new SudokuCell
			{
				Number = Number,
				Solution = Solution,
				IsReadOnly = IsReadOnly
			};

			foreach (int number in this._Possible)
			{
				cell._Possible.Add(number);
			}

			return cell;
		}

		internal bool ContainsPossible(int number) => this._Possible.Contains(number);
		public override bool Equals(Object obj) => this.Equals(obj as SudokuCell);
		public bool Equals(SudokuCell cell) => !(cell is null) && this.Number == cell.Number;
		public override int GetHashCode() => 8733563 * this.Number.GetHashCode();
		internal void MakeReadOnly() => this.IsReadOnly = true;
		internal bool RemovePossible(int number) => this._Possible.Remove(number);
		internal void ResetPossible() => this._Possible.Clear();

		public static bool operator ==(SudokuCell a, SudokuCell b) => a is null ? b is null : a.Equals(b);
		public static bool operator !=(SudokuCell a, SudokuCell b) => !(a == b);
	}
}
