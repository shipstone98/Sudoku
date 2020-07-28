using System;

namespace Sudoku.Android.ViewModels
{
	public class ControlEventArgs : EventArgs, IEquatable<ControlEventArgs>
	{
		public new static readonly ControlEventArgs Empty = new ControlEventArgs(ControlEvent.None, 0);

		public ControlEvent Event { get; }
		public int Number { get; }

		public ControlEventArgs(ControlEvent e, int number)
		{
			this.Event = e;
			this.Number = e == ControlEvent.Number ? number : 0;
		}

		public override bool Equals(Object obj) => this.Equals(obj as ControlEventArgs);

		public bool Equals(ControlEventArgs e)
		{
			if (e is null)
			{
				return false;
			}

			bool same = this.Event == e.Event;
			return same && (!same || this.Event != ControlEvent.Number || this.Number == e.Number);
		}

		public override int GetHashCode()
		{
			const int RANDOM = 452287;
			int hash = RANDOM * this.Event.GetHashCode();
			return this.Event == ControlEvent.Number ? hash * this.Number.GetHashCode() : hash * 0.GetHashCode();
		}

		public static bool operator ==(ControlEventArgs a, ControlEventArgs b) => a is null ? b is null : a.Equals(b);
		public static bool operator !=(ControlEventArgs a, ControlEventArgs b) => !(a == b);
	}
}