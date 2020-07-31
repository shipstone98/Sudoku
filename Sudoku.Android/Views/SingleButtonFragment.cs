using System;

using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

namespace Sudoku.Android.Views
{

	public class SingleButtonFragment : Fragment
	{
		private String _Text;
		private readonly Object LockObject;

		private event EventHandler _Click;

		private Button Button { get; set; }

		public String Text
		{
			get => this._Text;
			
			set
			{
				this._Text = value ?? String.Empty;

				if (!(this.Button is null))
				{
					this.Button.Text = this._Text;
				}
			}
		}

		public event EventHandler Click
		{
			add
			{
				lock (this.LockObject)
				{
					this._Click += value;
				}
			}

			remove
			{
				lock (this.LockObject)
				{
					this._Click -= value;
				}
			}
		}

		public SingleButtonFragment() : this(String.Empty) { }

		public SingleButtonFragment(String text)
		{
			this._Text = text ?? String.Empty;
			this.LockObject = new Object();
		}

		private void Clicked()
		{
			if (!(this._Click is null))
			{
				this._Click(this, EventArgs.Empty);
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			this.Button = this.View.FindViewById<Button>(Resource.Id.single_button);
			this.Button.Text = this._Text;
			this.Button.SetOnClickListener(new OnClickListener(this));
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_single_button, container, false);
		
		private class OnClickListener : Java.Lang.Object, View.IOnClickListener
		{
			private SingleButtonFragment Fragment { get; }

			internal OnClickListener(SingleButtonFragment fragment) => this.Fragment = fragment;

			public void OnClick(View v)
			{
				if (!(this.Fragment is null))
				{
					this.Fragment.Clicked();
				}
			}
		}
	}
}