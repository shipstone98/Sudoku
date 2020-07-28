using System;

using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

using Sudoku.Android.ViewModels;

namespace Sudoku.Android.Views
{
	public class ControlFragmentOnClickListener : Java.Lang.Object, View.IOnClickListener
	{
		public ControlFragment Fragment { get; set; }

		public void OnClick(View v)
		{
			if (this.Fragment is null)
			{
				return;
			}

			try
			{
				Button button = (Button) v;

				switch (button.Text)
				{
					case "Clear":
						//this.Fragment.State = new ControlEventArgs(ControlEvent.Clear, 0);
						this.Fragment.Update(ControlEvent.Clear, 0);
						return;
					case "Check":
						//this.Fragment.State = new ControlEventArgs(ControlEvent.Check, 0);
						this.Fragment.Update(ControlEvent.Check, 0);
						return;
					case "Notes":
						//this.Fragment.State = new ControlEventArgs(ControlEvent.Notes, 0);
						this.Fragment.Update(ControlEvent.Notes, 0);
						return;
					default:
						//this.Fragment.State = new ControlEventArgs(ControlEvent.Number, Int32.Parse(button.Text));
						this.Fragment.Update(ControlEvent.Number, Int32.Parse(button.Text));
						return;
				}
			}

			catch
			{
				return;
			}
		}
	}

	public class ControlFragment : Fragment
	{
		private const int Size = 9;

		private ControlEventArgs _State;
		private readonly Object LockObject;

		private event ControlEventHandler _Changed;

		private bool ActivityCreated { get; set; }
		private Button[] Buttons { get; }
		private Button CheckButton { get; set; }
		private Button ClearButton { get; set; }
		private Button NotesButton { get; set; }

		public ControlEventArgs State
		{
			get => this._State;

			set
			{
				this._State = value ?? ControlEventArgs.Empty;
				this.Update();
			}
		}

		public event ControlEventHandler Changed
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

		public ControlFragment()
		{
			this.LockObject = new Object();
			this._State = ControlEventArgs.Empty;
			this._Changed = null;
			this.ActivityCreated = false;
			this.Buttons = new Button[ControlFragment.Size];
			this.CheckButton = this.ClearButton = this.NotesButton = null;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			ControlFragmentOnClickListener listener = new ControlFragmentOnClickListener
			{
				Fragment = this
			};

			Button oneButton = this.View.FindViewById<Button>(Resource.Id.one_button);
			Button twoButton = this.View.FindViewById<Button>(Resource.Id.two_button);
			Button threeButton = this.View.FindViewById<Button>(Resource.Id.three_button);
			Button fourButton = this.View.FindViewById<Button>(Resource.Id.four_button);
			Button fiveButton = this.View.FindViewById<Button>(Resource.Id.five_button);
			Button sixButton = this.View.FindViewById<Button>(Resource.Id.six_button);
			Button sevenButton = this.View.FindViewById<Button>(Resource.Id.seven_button);
			Button eightButton = this.View.FindViewById<Button>(Resource.Id.eight_button);
			Button nineButton = this.View.FindViewById<Button>(Resource.Id.nine_button);
			this.NotesButton = this.View.FindViewById<Button>(Resource.Id.notes_button);
			this.ClearButton = this.View.FindViewById<Button>(Resource.Id.clear_button);
			this.CheckButton = this.View.FindViewById<Button>(Resource.Id.check_button);
			oneButton.SetOnClickListener(listener);
			twoButton.SetOnClickListener(listener);
			threeButton.SetOnClickListener(listener);
			fourButton.SetOnClickListener(listener);
			fiveButton.SetOnClickListener(listener);
			sixButton.SetOnClickListener(listener);
			sevenButton.SetOnClickListener(listener);
			eightButton.SetOnClickListener(listener);
			nineButton.SetOnClickListener(listener);
			this.CheckButton.SetOnClickListener(listener);
			this.ClearButton.SetOnClickListener(listener);
			this.NotesButton.SetOnClickListener(listener);
			this.Buttons[0] = oneButton;
			this.Buttons[1] = twoButton;
			this.Buttons[2] = threeButton;
			this.Buttons[3] = fourButton;
			this.Buttons[4] = fiveButton;
			this.Buttons[5] = sixButton;
			this.Buttons[6] = sevenButton;
			this.Buttons[7] = eightButton;
			this.Buttons[8] = nineButton;
			this.ActivityCreated = true;
			this.Update();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_control, container, false);

		private void Update()
		{
			if (!this.ActivityCreated)
			{
				return;
			}

			for (int i = 0; i < ControlFragment.Size; i ++)
			{
				this.Buttons[i].BackgroundTintList = ColorStateList.ValueOf(Color.Gray);
			}

			if (this.State.Event == ControlEvent.Number)
			{
				this.Buttons[this.State.Number - 1].BackgroundTintList = ColorStateList.ValueOf(Color.Blue);
			}
		}

		internal void Update(ControlEvent e, int number)
		{
			this._State = new ControlEventArgs(e, number);

			if (!(this._Changed is null))
			{
				this._Changed(this, this._State);
			}

			this.Update();
		}
	}
}