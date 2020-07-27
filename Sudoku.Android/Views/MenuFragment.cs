using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

namespace Sudoku.Android.Views
{
	public class ButtonOnClickListener : Java.Lang.Object, View.IOnClickListener
	{
		public const String GenerateAction = "Generate";

		public String Action { get; }
		public FragmentActivity Activity { get; }

		public ButtonOnClickListener(String action, FragmentActivity activity)
		{
			this.Action = action;
			this.Activity = activity;
		}

		public void OnClick(View view)
		{
			switch (this.Action)
			{
				case ButtonOnClickListener.GenerateAction:
					Intent intent = new Intent(this.Activity.ApplicationContext, Java.Lang.Class.FromType(typeof (SudokuActivity)));
					this.Activity.StartActivity(intent);
					//Open new activity
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}

	public class MenuFragment : Fragment
	{
		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Button generateButton = this.View.FindViewById<Button>(Resource.Id.new_button);
			generateButton.SetOnClickListener(new ButtonOnClickListener(ButtonOnClickListener.GenerateAction, this.Activity));
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_menu, container, false);
	}
}