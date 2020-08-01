using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

using Java.IO;

using Sudoku.Android.ViewModels;

namespace Sudoku.Android.Views
{
	public class MenuFragmentOnClickListener : Java.Lang.Object, View.IOnClickListener
	{
		public ActionType Action { get; }
		public FragmentActivity Activity { get; private set; }

		public MenuFragmentOnClickListener(ActionType action, FragmentActivity activity)
		{
			this.Action = action;
			this.Activity = activity;
		}

		public void OnClick(View view)
		{
			if (this.Activity is null)
			{
				return;
			}

			Intent intent;

			switch (this.Action)
			{
				case ActionType.About:
					intent = new Intent(this.Activity.ApplicationContext, Java.Lang.Class.FromType(typeof (AboutActivity)));
					break;
				case ActionType.Continue:
					try
					{
						File file = new File(this.Activity.FilesDir, "Sudoku.txt");

						if (file.Exists())
						{
							intent = new Intent(this.Activity.ApplicationContext, Java.Lang.Class.FromType(typeof (SudokuActivity)));
							intent.PutExtra("action", (int) ActionType.Continue);
							break;
						}

						else
						{
							throw new Exception();
						}
					}

					catch
					{
						Toast toast = Toast.MakeText(this.Activity, "There was no saved puzzle to continue.", ToastLength.Long);
						toast.Show();
						return;
					}

				case ActionType.Generate:
					intent = new Intent(this.Activity.ApplicationContext, Java.Lang.Class.FromType(typeof (SudokuActivity)));
					intent.PutExtra("action", (int) ActionType.Generate);
					break;
				case ActionType.New:
					intent = new Intent(this.Activity.ApplicationContext, Java.Lang.Class.FromType(typeof (DifficultyActivity)));
					intent.PutExtra("action", (int) ActionType.New);
					break;
				default:
					throw new NotImplementedException();
			}

			this.Activity.StartActivity(intent);
		}
	}

	public class MenuFragment : Fragment
	{
		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Button generateButton = this.View.FindViewById<Button>(Resource.Id.generate_button);
			generateButton.SetOnClickListener(new MenuFragmentOnClickListener(ActionType.Generate, this.Activity));
			Button newButton = this.View.FindViewById<Button>(Resource.Id.new_button);
			newButton.SetOnClickListener(new MenuFragmentOnClickListener(ActionType.New, this.Activity));
			Button continueButton = this.View.FindViewById<Button>(Resource.Id.continue_button);
			continueButton.SetOnClickListener(new MenuFragmentOnClickListener(ActionType.Continue, this.Activity));
			Button aboutButton = this.View.FindViewById<Button>(Resource.Id.about_button);
			aboutButton.SetOnClickListener(new MenuFragmentOnClickListener(ActionType.About, this.Activity));
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_menu, container, false);
	}
}