﻿using System;

using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Util;
using Sudoku.Android.ViewModels;

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

		private event SudokuViewEventHandler _Changed;

		private Paint CellText { get; }
		private Paint IncorrectCellText { get; }
		private Paint InnerBorder { get; }
		private Paint MatchingFill { get; }
		private Paint NeighbourFill { get; }
		private Paint OuterBorder { get; }
		private Paint ReadOnlyCellText { get; }
		private Paint SelectedFill { get; }

		public int Column { get; private set; }
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

		public event SudokuViewEventHandler Changed
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
					this.Changed -= value;
				}
			}
		}

		public SudokuView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
		{
			//TypedArray attributes = context.Theme.ObtainStyledAttributes(attributeSet, Resource.Styleable.Sudoku)

			this._Sudoku = null;
			this.LockObject = new Object();

			this._Changed = null;

			this.Column = SudokuView.DefaultColumn;
			this.Pixels = SudokuView.DefaultPixels;
			this.Row = SudokuView.DefaultRow;
			this.SelectedNumber = 0;
			this.Size = SudokuView.DefaultSize;

			this.CellText = new Paint();
			this.IncorrectCellText = new Paint();
			this.InnerBorder = new Paint();
			this.MatchingFill = new Paint();
			this.NeighbourFill = new Paint();
			this.OuterBorder = new Paint();
			this.ReadOnlyCellText = new Paint();
			this.SelectedFill = new Paint();

			this.CellText.Color = Color.Black;
			this.CellText.TextSize = SudokuView.DefaultTextSize;
			this.CellText.SetStyle(Paint.Style.FillAndStroke);
			this.IncorrectCellText.Color = Color.Red;
			this.IncorrectCellText.TextSize = SudokuView.DefaultTextSize;
			this.IncorrectCellText.SetStyle(Paint.Style.FillAndStroke);
			this.InnerBorder.Color = Color.Black;
			this.InnerBorder.StrokeWidth = SudokuView.DefaultStrokeWidth;
			this.InnerBorder.SetStyle(Paint.Style.Stroke);
			this.MatchingFill.Color = Color.Rgb(220, 230, 140);
			this.MatchingFill.SetStyle(Paint.Style.FillAndStroke);
			this.NeighbourFill.Color = Color.Rgb(220, 240, 255);
			this.NeighbourFill.SetStyle(Paint.Style.FillAndStroke);
			this.OuterBorder.Color = Color.Black;
			this.OuterBorder.StrokeWidth = SudokuView.DefaultStrokeWidth * SudokuView.OuterBorderRatio;
			this.OuterBorder.SetStyle(Paint.Style.Stroke);
			this.ReadOnlyCellText.Color = Color.DarkBlue;
			this.ReadOnlyCellText.TextSize = SudokuView.DefaultTextSize;
			this.ReadOnlyCellText.SetStyle(Paint.Style.FillAndStroke);
			this.SelectedFill.Color = Color.Rgb(220, 220, 255);
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