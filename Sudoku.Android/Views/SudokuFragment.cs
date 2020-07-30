using System;

using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.Fragment.App;
using AndroidX.Lifecycle;

using Sudoku.Android.ViewModels;
using Sudoku.Android.Views.Custom;

namespace Sudoku.Android.Views
{
	public class SudokuFragment : Fragment
	{
		private SudokuViewModel ViewModel { get; set; }

		private static String FormatTime(double ms)
		{
			int s = (int) (ms / 1000);
			ms -= s * 1000;

			if (s < 60)
			{
				return $"{s}s {ms}ms";
			}

			int m = (int) ((double) s / 60);
			s -= m * 60;

			if (m < 60)
			{
				return $"{m}m {s}s {ms}ms";
			}

			int h = (int) ((double) m / 60);
			m -= h * 60;
			return $"{h}h {m}m {s}s {ms}ms";
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			this.ViewModel = (SudokuViewModel) new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof (SudokuViewModel)));
			SudokuView sudokuView = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			ControlFragment controlFragment = this.ChildFragmentManager.FindFragmentById(Resource.Id.control_fragment) as ControlFragment;
			sudokuView.Sudoku = this.ViewModel.Sudoku;
			controlFragment.State = this.ViewModel.State;
			controlFragment.Changed += new ControlEventHandler((sender, e) => this.ViewModel.State = e);
			sudokuView.Changed += new SudokuViewEventHandler((sender, e) => this.ViewModel.SetRowAndColumn(e.Row, e.Column));
			this.ViewModel.StateChanged += new ControlEventHandler((sender, e) => controlFragment.State = e);
			this.ViewModel.StateChanged += new ControlEventHandler((sender, e) => sudokuView.SetRowAndColumn(this.ViewModel.Row, this.ViewModel.Column));
			this.ViewModel.Completed += new EventHandler(this.OnCompletion);
		}

		private void OnCompletion(Object sender, EventArgs e)
		{
			if (this.ViewModel.IsCorrect)
			{
				//Disable custom view and control fragment
				Toast toast = Toast.MakeText(this.Context, $"Congratulations: Puzzle completed in {SudokuFragment.FormatTime(this.ViewModel.TimeOnStop)} using {this.ViewModel.MoveCount} moves.", ToastLength.Long);
				toast.Show();
				//Check and modify high scores
			}

			else
			{
				Toast toast = Toast.MakeText(this.Context, "Uh-oh. Looks like you made some mistakes...", ToastLength.Long);
				toast.Show();
				//Highlight incorrect cells
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_sudoku, container, false);

		public override void OnPause()
		{
			this.ViewModel.Pause();
			base.OnPause();
		}

		public override void OnResume()
		{
			base.OnResume();
			this.ViewModel.Resume();
		}
	}
}