using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;

using Java.Lang;

using Sudoku.Android.ViewModels;

namespace Sudoku.Android.Views
{
	public class DifficultyFragmentOnClickListener : Java.Lang.Object, View.IOnClickListener
	{
		public DifficultyFragment Fragment { get; private set; }

		public DifficultyFragmentOnClickListener(DifficultyFragment fragment) => this.Fragment = fragment;

		public void OnClick(View view)
		{
			if (this.Fragment is null)
			{
				return;
			}

			Intent intent = new Intent(this.Fragment.Context, Java.Lang.Class.FromType(typeof (SudokuActivity)));
			intent.PutExtra("difficulty", this.Fragment.View.FindViewById<Spinner>(Resource.Id.difficulty_spinner).SelectedItemPosition + 1);
			ActionType action = (ActionType) (int) this.Fragment.Activity.Intent.Extras.Get("action");
			intent.PutExtra("action", (int) action);
			this.Fragment.StartActivity(intent);
			DifficultyFragment fragment = this.Fragment;
			this.Fragment = null;
			fragment.Activity.Finish();
		}
	}

	public class DifficultyFragment : Fragment
	{
		private DifficultyFragmentOnClickListener Listener { get; set; }

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Spinner difficultySpinner = this.View.FindViewById<Spinner>(Resource.Id.difficulty_spinner);
			ArrayAdapter<ICharSequence> adapter = ArrayAdapter<ICharSequence>.CreateFromResource(this.Context, Resource.Array.difficulty_names, global::Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource(global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			difficultySpinner.Adapter = adapter;
			Button goButton = this.View.FindViewById<Button>(Resource.Id.go_button);
			difficultySpinner.SetSelection(savedInstanceState is null ? 0 : savedInstanceState.GetInt("selection", 0));
			this.Listener = new DifficultyFragmentOnClickListener(this);
			goButton.SetOnClickListener(this.Listener);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_difficulty, container, false);

		public override void OnSaveInstanceState(Bundle outState)
		{
			Spinner difficultySpinner = this.View.FindViewById<Spinner>(Resource.Id.difficulty_spinner);
			outState.PutInt("selection", difficultySpinner.SelectedItemPosition);
			base.OnSaveInstanceState(outState);
		}
	}
}