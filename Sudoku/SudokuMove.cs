using System;

namespace Sudoku
{
    public class SudokuMove
    {
        public const String DefaultNumberSeparator = ",";
        public const int DefaultPadding = 16;
        public const String DefaultSeparator = "\t";

        private readonly int[] _Columns;
        private readonly int[] _Numbers;
        private readonly int[] _Possible;
        private readonly int[] _Rows;

        public int[] Columns
		{
            get
			{
                int[] columns = new int[this._Columns.Length];
                Array.Copy(this._Columns, columns, columns.Length);
                return columns;
			}
		}

        public int[] Numbers
        {
            get
            {
                int[] numbers = new int[this._Numbers.Length];
                Array.Copy(this._Numbers, numbers, numbers.Length);
                return numbers;
            }
        }

        public SudokuPattern Pattern { get; }

        public String PatternName
        {
            get
            {
                switch (this.Pattern)
                {
                    case SudokuPattern.ClaimingCandidate:
                        return "Claiming Candidate";
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

        public int[] Possible
		{
            get
            {
                int[] possible = new int[this._Possible.Length];
                Array.Copy(this._Possible, possible, possible.Length);
                return possible;
            }
        }

        public int[] Rows
		{

            get
            {
                int[] rows = new int[this._Rows.Length];
                Array.Copy(this._Rows, rows, rows.Length);
                return rows;
            }
        }

        public SudokuMove(int[] rows, int[] columns, int[] numbers, SudokuPattern pattern, int[] possible)
		{
            if (rows is null)
			{
                throw new ArgumentNullException(nameof (rows));
			}

            if (columns is null)
			{
                throw new ArgumentNullException(nameof (columns));
			}

            if (numbers is null)
			{
                throw new ArgumentNullException(nameof (numbers));
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

            this._Columns = columns;
            this._Numbers = numbers;
            this._Possible = possible;
            this._Rows = rows;
            this.Pattern = pattern;
		}

        public SudokuMove(int row, int column, int number, SudokuPattern pattern, int[] possible) : this(new int[] { row }, new int[] { column }, new int[] { number }, pattern, possible)
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
        }

        public override String ToString() => this.ToString(SudokuMove.DefaultPadding);
        public String ToString(int padding) => this.ToString(SudokuMove.DefaultPadding, SudokuMove.DefaultSeparator);

        public String ToString(int padding, String separator, String numberSeparator = SudokuMove.DefaultNumberSeparator)
        {
            if (padding < 0)
			{
                throw new ArgumentOutOfRangeException(nameof (padding));
			}

            int[] rows = new int[this._Rows.Length];
            int[] columns = new int[this._Columns.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = this._Rows[i] + 1;
            }

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = this._Columns[i] + 1;
            }

            String rowString = String.Join(numberSeparator, rows).PadRight(padding);
            String columnString = String.Join(numberSeparator, columns).PadRight(padding);
            String numberString = String.Join(numberSeparator, this._Numbers).PadRight(padding);
            String possibleString = String.Join(numberSeparator, this._Possible).PadRight(padding);
            return String.Join(separator, rowString, columnString, numberString, this.PatternName.PadRight(padding), possibleString);
        }
    }
}