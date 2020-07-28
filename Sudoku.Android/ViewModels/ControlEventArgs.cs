using System;

namespace Sudoku.Android.ViewModels
{
	public class ControlEventArgs : EventArgs
	{
		public static readonly ControlEventArgs Empty = new ControlEventArgs(ControlEvent.None, 0);

		public ControlEvent Event { get; }
		public int Number { get; }

		public ControlEventArgs(ControlEvent e, int number)
		{
			this.Event = e;
			this.Number = e == ControlEvent.Number ? number : 0;
		}
	}
}