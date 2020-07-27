using System;
using System.Linq;

namespace Sudoku
{
    /// <summary>
    /// Represents a strategy-based move used to solve <see cref="Sudoku"/> puzzles.
    /// </summary>
    public class SudokuMove
    {
        private const String DefaultNumberSeparator = ",";
        private const int DefaultPadding = 16;
        private const String DefaultSeparator = "\t";

        private readonly int[] _Columns;
        private readonly int[] _Numbers;
        private readonly int[] _Possible;
        private readonly int[] _Rows;

        /// <summary>
        /// Gets a new array containing the zero-based columns of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.
        /// </summary>
        public int[] Columns
		{
            get
			{
                int[] columns = new int[this._Columns.Length];
                Array.Copy(this._Columns, columns, columns.Length);
                return columns;
			}
		}

        /// <summary>
        /// Gets a new array containing the numbers used in the <see cref="SudokuMove"/>.
        /// </summary>
        public int[] Numbers
        {
            get
            {
                int[] numbers = new int[this._Numbers.Length];
                Array.Copy(this._Numbers, numbers, numbers.Length);
                return numbers;
            }
        }

        /// <summary>
        /// Gets the pattern used in the <see cref="SudokuMove"/>.
        /// </summary>
        public SudokuPattern Pattern { get; }

        /// <summary>
        /// Gets a string representation of the pattern used in the <see cref="SudokuMove"/>.
        /// </summary>
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

        /// <summary>
        /// Gets a new array containing the numbers that were possible instantly before the <see cref="SudokuMove"/> move was made.
        /// </summary>
        public int[] Possible
		{
            get
            {
                int[] possible = new int[this._Possible.Length];
                Array.Copy(this._Possible, possible, possible.Length);
                return possible;
            }
        }

        /// <summary>
        /// Gets a new array containing the zero-based rows of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.
        /// </summary>
        public int[] Rows
		{

            get
            {
                int[] rows = new int[this._Rows.Length];
                Array.Copy(this._Rows, rows, rows.Length);
                return rows;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuMove"/> class.
        /// </summary>
        /// <param name="rows">An array containing the zero-based rows of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.</param>
        /// <param name="columns">An array containing the zero-based columns of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.</param>
        /// <param name="numbers">An array containing the numbers used in the <see cref="SudokuMove"/>.</param>
        /// <param name="pattern">The pattern used in the <see cref="SudokuMove"/>.</param>
        /// <param name="possible">An array containing the numbers that were possible instantly before the <see cref="SudokuMove"/> move was made.</param>
        /// <exception cref="ArgumentException">The lengths of <c><paramref name="rows"/></c>, <c><paramref name="columns"/></c>, <c><paramref name="numbers"/></c> or <c><paramref name="possible"/></c> are equal to 0, or any of their items are less than 0 - or - the length of <c><paramref name="numbers"/></c> is greater than the length of <c><paramref name="possible"/></c> - or - <c><paramref name="pattern"/></c> is equal to <see cref="SudokuPattern.None"/> - or - <c><paramref name="possible"/></c> does not contain all values in <c><paramref name="numbers"/></c>.</exception>
        /// <exception cref="ArgumentNullException"><c><paramref name="rows"/></c>, <c><paramref name="columns"/></c>, <c><paramref name="numbers"/></c> or <c><paramref name="possible"/></c> is <c>null</c>.</exception>
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

            if (rows.Length == 0)
			{
                throw new ArgumentException(nameof (rows));
			}

            if (columns.Length == 0)
			{
                throw new ArgumentException(nameof (columns));
			}

            if (numbers.Length == 0)
			{
                throw new ArgumentException(nameof (numbers));
			}

            if (possible.Length == 0)
            {
                throw new ArgumentException(nameof (possible));
            }

            for (int i = 0; i < rows.Length; i ++)
			{
                if (rows[i] < 0)
				{
                    throw new ArgumentException(nameof (rows));
				}
			}

            for (int i = 0; i < columns.Length; i ++)
			{
                if (columns[i] < 0)
				{
                    throw new ArgumentException(nameof (columns));
				}
			}

            for (int i = 0; i < numbers.Length; i ++)
			{
                if (numbers[i] < 0 || !possible.Contains(numbers[i]))
				{
                    throw new ArgumentException(nameof (numbers));
				}
			}

            for (int i = 0; i < possible.Length; i ++)
			{
                if (possible[i] < 0)
				{
                    throw new ArgumentException(nameof (possible));
				}
			}

            if (possible.Length < numbers.Length)
			{
                throw new ArgumentException(nameof (numbers));
			}

            this._Columns = columns;
            this._Numbers = numbers;
            this._Possible = possible;
            this._Rows = rows;
            this.Pattern = pattern;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuMove"/> class.
        /// </summary>
        /// <param name="row">The zero-based row of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.</param>
        /// <param name="column">The zero-based column of the <see cref="Sudoku"/> puzzle affected by the <see cref="SudokuMove"/>.</param>
        /// <param name="number">The number used in the <see cref="SudokuMove"/>.</param>
        /// <param name="pattern">The pattern used in the <see cref="SudokuMove"/>.</param>
        /// <param name="possible">An array containing the numbers that were possible instantly before the <see cref="SudokuMove"/> move was made.</param>
        /// <exception cref="ArgumentException">The length of <c><paramref name="possible"/></c> is equal to 0, or any of its items are less than 0 - or - <c><paramref name="pattern"/></c> is equal to <see cref="SudokuPattern.None"/> - or - <c><paramref name="possible"/></c> does not contain <c><paramref name="number"/></c>.</exception>
        /// <exception cref="ArgumentNullException"><c><paramref name="possible"/></c> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="row"/></c>, <c><paramref name="column"/></c> or <c><paramref name="number"/></c> is less than 0.</exception>
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

            if (!possible.Contains(number))
			{
                throw new ArgumentException(nameof (number));
			}

            this._Columns = new int[] { column };
            this._Numbers = new int[] { number };
            this._Possible = possible;
            this._Rows = new int[] { row };
            this.Pattern = pattern;
		}

        /// <summary>
        /// Returns a string that represents the current <see cref="SudokuMove"/> with default padding and separators.
        /// </summary>
        /// <returns>A string that represents the current <see cref="SudokuMove"/>.</returns>
        public override String ToString() => this.ToString(SudokuMove.DefaultPadding);

        /// <summary>
        /// Returns a string that represents the current <see cref="SudokuMove"/> with the specified <c><paramref name="padding"/></c> and default separators.
        /// </summary>
        /// <param name="padding">The number of characters in each component of the resulting string, equal to the number of original characters plus any additional padding characters.</param>
        /// <returns>A string that represents the current <see cref="SudokuMove"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="padding"/></c> is less than 0.</exception>
        public String ToString(int padding) => this.ToString(padding, SudokuMove.DefaultSeparator);

        /// <summary>
        /// Returns a string that represents the current <see cref="SudokuMove"/> with the specified <c><paramref name="padding"/></c> and separators.
        /// </summary>
        /// <param name="padding">The number of characters in each component of the resulting string, equal to the number of original characters plus any additional padding characters.</param>
        /// <param name="separator">A separator to use between components of the resulting string.</param>
        /// <param name="numberSeparator">An optional separator to use between any numbers in the resulting string.</param>
        /// <returns>A string that represents the current <see cref="SudokuMove"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="padding"/></c> is less than 0.</exception>
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