using System;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Util;
using Sudoku.Android.ViewModels;
using System.Text.RegularExpressions;

namespace Sudoku.Android.Views.Custom
{
	public class SudokuView : View
	{
		private const int DefaultColumn = -1;
		private const float DefaultPixels = 0F;
		private const int DefaultRow = -1;
		private const int DefaultSize = 9;
		private const float DefaultStrokeWidth = 2F;
		private const float DefaultTextSize = 40F;
		private const int OuterBorderRatio = 3;

		private SudokuPuzzle _Sudoku;
		private readonly Object LockObject;

		private event EventHandler<SudokuViewEventArgs> _Changed;

		private Paint CellText { get; }
		private Paint IncorrectCellText { get; }
		private Paint InnerBorder { get; }
		private Paint MatchingFill { get; }
		private Paint NeighbourFill { get; }
		private Paint OuterBorder { get; }
		private Paint ReadOnlyCellText { get; }
		private Paint SelectedFill { get; }

		public int Column { get; private set; }
		public bool DisableMatchingCellFill { get; set; }
		private float Pixels { get; set; }
		public int Row { get; private set; }
		public int SelectedNumber { get; set; }
		public int Size { get; private set; }

		public SudokuPuzzle Sudoku
		{
			get => this._Sudoku;

			set
			{
				this._Sudoku = value;
				this.Size = value is null ? SudokuView.DefaultSize : value.Size;
				this.Invalidate();
				this.RequestLayout();
			}
		}

		public event EventHandler<SudokuViewEventArgs> Changed
		{
			add
			{
				lock (this.LockObject)
				{
					this._Changed += value;
				}
			}

			remove
			{
				lock (this.LockObject)
				{
					this._Changed -= value;
				}
			}
		}

		public SudokuView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
		{
			TypedArray attributes = context.Theme.ObtainStyledAttributes(attributeSet, Resource.Styleable.SudokuView, 0, 0);
			this.Size = attributes.GetInteger(Resource.Styleable.SudokuView_size, SudokuView.DefaultSize);
			this.Size = SudokuPuzzle.VerifySize(this.Size) ? this.Size : SudokuView.DefaultSize;
			this.Row = SudokuView.DefaultRow;
			this.Column = SudokuView.DefaultColumn;
			this.DisableMatchingCellFill = attributes.GetBoolean(Resource.Styleable.SudokuView_disable_matching_cell_fill, false);

			this._Sudoku = null;
			this.LockObject = new Object();

			this._Changed = null;

			this.Pixels = SudokuView.DefaultPixels;
			this.SelectedNumber = 0;

			this.CellText = new Paint();
			this.IncorrectCellText = new Paint();
			this.InnerBorder = new Paint();
			this.MatchingFill = new Paint();
			this.NeighbourFill = new Paint();
			this.OuterBorder = new Paint();
			this.ReadOnlyCellText = new Paint();
			this.SelectedFill = new Paint();

			float textSize = attributes.GetFloat(Resource.Styleable.SudokuView_text_size, SudokuView.DefaultTextSize);
			Color borderColor = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_border_color), Color.Black);
			this.CellText.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_cell_text_color), Color.Black);
			this.CellText.TextSize = textSize;
			this.CellText.SetStyle(Paint.Style.FillAndStroke);
			this.IncorrectCellText.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_incorrect_cell_text_color), Color.Red);
			this.IncorrectCellText.TextSize = textSize;
			this.IncorrectCellText.SetStyle(Paint.Style.FillAndStroke);
			this.InnerBorder.Color = borderColor;
			this.InnerBorder.StrokeWidth = attributes.GetFloat(Resource.Styleable.SudokuView_inner_border_thickness, SudokuView.DefaultStrokeWidth);
			this.InnerBorder.SetStyle(Paint.Style.Stroke);
			this.MatchingFill.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_matching_cell_color), Color.Rgb(220, 230, 140));
			this.MatchingFill.SetStyle(Paint.Style.FillAndStroke);
			this.NeighbourFill.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_neighbour_cell_color), Color.Rgb(220, 240, 255));
			this.NeighbourFill.SetStyle(Paint.Style.FillAndStroke);
			this.OuterBorder.Color = borderColor;
			this.OuterBorder.StrokeWidth = attributes.GetFloat(Resource.Styleable.SudokuView_inner_border_thickness, SudokuView.DefaultStrokeWidth * SudokuView.OuterBorderRatio);
			this.OuterBorder.SetStyle(Paint.Style.Stroke);
			this.ReadOnlyCellText.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_readonly_cell_text_color), Color.Blue);
			this.ReadOnlyCellText.TextSize = textSize;
			this.ReadOnlyCellText.SetStyle(Paint.Style.FillAndStroke);
			this.SelectedFill.Color = SudokuView.ParseColor(attributes.GetString(Resource.Styleable.SudokuView_selected_cell_color), Color.Rgb(220, 220, 255));
			this.SelectedFill.SetStyle(Paint.Style.FillAndStroke);
		}

		private void DrawBorders(Canvas canvas)
		{
			int sqrt = (int) Math.Sqrt(this.Size);

			for (int i = 0; i <= this.Size; i ++)
			{
				Paint paint = i % sqrt == 0 ? this.OuterBorder : this.InnerBorder;
				float coordinate = i * this.Pixels;
				canvas.DrawLine(coordinate, 0, coordinate, this.Height, paint);
				canvas.DrawLine(0, coordinate, this.Width, coordinate, paint);
			}
		}

		private void FillCell(Canvas canvas, int row, int column, Paint paint) => canvas.DrawRect(column * this.Pixels, row * this.Pixels, (column + 1) * this.Pixels, (row + 1) * this.Pixels, paint);

		private void FillCells(Canvas canvas)
		{
			if (this.Sudoku is null)
			{
				return;
			}

			if (this.Column < 0 || this.Row < 0)
			{
				if (this.SelectedNumber > 0) //Highlights selected if no cell selected but number pressed
				{
					for (int i = 0; i < this.Sudoku.Size; i ++)
					{
						for (int j = 0; j < this.Sudoku.Size; j ++)
						{
							if (this.Sudoku[i, j] == this.SelectedNumber)
							{
								this.FillCell(canvas, i, j, this.MatchingFill);
							}
						}
					}
				}

				return;
			}

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				this.FillCell(canvas, this.Row, i, this.NeighbourFill);
				this.FillCell(canvas, i, this.Column, this.NeighbourFill);
			}

			int row = this.Row - this.Row % this.Sudoku.BlockSize;
			int column = this.Column - this.Column % this.Sudoku.BlockSize;

			for (int i = 0; i < this.Sudoku.BlockSize; i ++)
			{
				int currentRow = row + i;

				for (int j = 0; j < this.Sudoku.BlockSize; j ++)
				{
					this.FillCell(canvas, currentRow, column + j, this.NeighbourFill);
				}
			}

			if (!this.DisableMatchingCellFill)
			{
				for (int i = 0; i < this.Sudoku.Size; i++)
				{
					for (int j = 0; j < this.Sudoku.Size; j++)
					{
						if (this.Sudoku[i, j] != 0 && this.Sudoku[i, j] == this.Sudoku[this.Row, this.Column])
						{
							this.FillCell(canvas, i, j, this.MatchingFill);
						}
					}
				}
			}

			this.FillCell(canvas, this.Row, this.Column, this.SelectedFill);
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			if (this.Sudoku is null)
			{
				return;
			}

			this.Pixels = ((float) this.Width) / this.Sudoku.Size;
			float textSize = this.Pixels / 3F;
			this.IncorrectCellText.TextSize = this.ReadOnlyCellText.TextSize = this.CellText.TextSize = textSize;
			this.FillCells(canvas);
			this.DrawBorders(canvas);
			this.WriteCells(canvas);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			int constraint = Math.Min(widthMeasureSpec, heightMeasureSpec);
			this.SetMeasuredDimension(constraint, constraint);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (e.Action == MotionEventActions.Down)
			{
				int row = (int) (e.GetY() / this.Pixels);
				int column = (int) (e.GetX() / this.Pixels);
				this.SetRowAndColumn(row, column);

				if (!(this._Changed is null))
				{
					this._Changed(this, new SudokuViewEventArgs(row, column));
				}

				return true;
			}

			return false;
		}

		private static Color ParseColor(String text, Color defaultColor)
		{
			if (String.IsNullOrWhiteSpace(text))
			{
				return defaultColor;
			}

			try
			{
				return (Color) Enum.Parse(typeof (Color), text, true);
			}

			catch
			{
				text = Regex.Replace(text, @"\s+", "");
				String[] split = text.Split(',');

				if (split.Length == 3)
				{
					sbyte[] rgb = new sbyte[3];

					for (int i = 0; i < 3; i ++)
					{
						try
						{
							rgb[i] = SByte.Parse(split[0]);
						}

						catch
						{
							return defaultColor;
						}
					}

					return Color.Rgb(rgb[0], rgb[1], rgb[2]);
				}
			}

			return defaultColor;
		}

		public void SetRowAndColumn(int row, int column)
		{
			this.Row = row;
			this.Column = column;
			this.Invalidate();
			this.RequestLayout();
		}

		public void Update()
		{
			this.Invalidate();
			this.RequestLayout();
		}

		private void WriteCells(Canvas canvas)
		{
			if (this.Sudoku is null)
			{
				return;
			}

			for (int i = 0; i < this.Sudoku.Size; i ++)
			{
				float iPixels = i * this.Pixels;

				for (int j = 0; j < this.Sudoku.Size; j ++)
				{
					if (this.Sudoku[i, j] == 0)
					{
						continue;
					}

					Rect cell = new Rect();
					String number = this.Sudoku[i, j].ToString();
					this.CellText.GetTextBounds(number, 0, number.Length, cell);
					float width = this.CellText.MeasureText(number);
					float halfPixels = this.Pixels / 2;
					canvas.DrawText(number, (j * this.Pixels) + halfPixels - width / 2, iPixels + halfPixels - cell.Height() / 2, this.Sudoku.CheckReadOnly(i, j) ? this.ReadOnlyCellText : this.CellText);
				}
			}
		}
	}
}