using System;

using Android.Bluetooth;
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
			Button button = v as Button;

			if (this.Fragment is null)
			{
				return;
			}

			switch (button.Text)
			{
				case "Check":
					this.Fragment.State = new ControlEventArgs(ControlEvent.Check, 0);
					this.Fragment.Update();
					break;
				case "Clear":
					this.Fragment.State = new ControlEventArgs(ControlEvent.Clear, 0);
					this.Fragment.Update();
					break;
				case "Notes":
					this.Fragment.State = new ControlEventArgs(ControlEvent.Notes, 0);
					this.Fragment.Update();
					break;

				default:
					if (Int32.TryParse(button.Text, out int result))
					{
						this.Fragment.State = new ControlEventArgs(ControlEvent.Number, result);
						this.Fragment.Update();
					}

					break;
			}
		}
	}

	public class ControlFragment : Fragment
	{
		private const int Size = 9;

		internal ControlEventArgs _State;
		private readonly Object LockObject;

		private event ControlEventHandler _Changed;

		private bool ActivityCreated { get; set; }
		private Button[] NumberButtons { get; }
		private Button CheckButton { get; set; }
		private Button ClearButton { get; set; }
		private Button NotesButton { get; set; }

		public ControlEventArgs State
		{
			get => this._State;
			set => this.Update(value);
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
					this._Changed -= value;
				}
			}
		}

		public ControlFragment()
		{
			this._State = ControlEventArgs.Empty;
			this.LockObject = new Object();
			this._Changed = null;
			this.ActivityCreated = false;
			this.NumberButtons = new Button[ControlFragment.Size];
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
			this.NumberButtons[0] = oneButton;
			this.NumberButtons[1] = twoButton;
			this.NumberButtons[2] = threeButton;
			this.NumberButtons[3] = fourButton;
			this.NumberButtons[4] = fiveButton;
			this.NumberButtons[5] = sixButton;
			this.NumberButtons[6] = sevenButton;
			this.NumberButtons[7] = eightButton;
			this.NumberButtons[8] = nineButton;
			this.ActivityCreated = true;
			this.Update(this._State);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_control, container, false);

		internal void Update()
		{
			this.Update(this._State);

			if (!(this._Changed is null))
			{
				this._Changed(this, this._State);
			}
		}

		private void Update(ControlEventArgs state)
		{
			if (!this.ActivityCreated)
			{
				return;
			}

			this._State = state;
			ColorStateList gray = ColorStateList.ValueOf(Color.Gray);

			for (int i = 0; i < ControlFragment.Size; i ++)
			{
				this.NumberButtons[i].BackgroundTintList = gray;
			}

			this.CheckButton.BackgroundTintList = this.ClearButton.BackgroundTintList = this.NotesButton.BackgroundTintList = gray;
			Button selectedButton = null;

			switch (state.Event)
			{
				case ControlEvent.Check:
					selectedButton = this.CheckButton;
					break;
				case ControlEvent.Clear:
					selectedButton = this.ClearButton;
					break;
				case ControlEvent.Notes:
					selectedButton = this.NotesButton;
					break;
				case ControlEvent.Number:
					selectedButton = this.NumberButtons[state.Number - 1];
					break;
			}

			if (!(selectedButton is null))
			{
				selectedButton.BackgroundTintList = ColorStateList.ValueOf(Color.Blue);
			}
		}
	}
}