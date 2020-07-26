using System;

namespace Sudoku
{
    public class SudokuMove
    {
        public const int DefaultPatternNamePadding = 16;

        private readonly int[] _Possible;

        public int Column { get; }
        public int Number { get; }
        public SudokuPattern Pattern { get; }

        public String PatternName
        {
            get
            {
                switch (this.Pattern)
                {
                    case SudokuPattern.HiddenSingle:
                        return "Hidden Single";
                    case SudokuPattern.NakedSingle:
                        return "Naked Single";
                    case SudokuPattern.PointingCandidate:
                        return "Pointing Candidate";
                    default:
                        return String.Empty;
                }
            }
        }

        public int[] Possible => this._Possible;
        public int Row { get; }

        public SudokuMove(int row, int column, int number, SudokuPattern pattern, int[] possible)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(nameof (row));
            }

            if (column < 0)
            {
                throw new ArgumentOutOfRangeException(nameof (column));
            }

            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(nameof (number));
            }

            if (pattern == SudokuPattern.None)
            {
                throw new ArgumentException(nameof (pattern));
            }

            if (possible is null)
            {
                throw new ArgumentNullException(nameof (possible));
            }

            if (possible.Length == 0)
            {
                throw new ArgumentException(nameof (possible));
            }
            
            this._Possible = possible;
            this.Column = column;
            this.Number = number;
            this.Pattern = pattern;
            this.Row = row;
        }

        public override String ToString() => this.ToString(SudokuMove.DefaultPatternNamePadding);
        public virtual String ToString(int patternNamePadding, bool zeroSpace = false) => String.Join("\t", this.Row + 1, this.Column + 1, this.Number == 0 && zeroSpace ? " " : this.Number.ToString(), this.PatternName.PadRight(patternNamePadding), String.Join<int>(",", this.Possible).PadRight(patternNamePadding));
    }
}