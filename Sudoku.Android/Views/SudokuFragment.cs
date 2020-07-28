using System;

using Android.OS;
using Android.Views;

using AndroidX.Fragment.App;
using AndroidX.Lifecycle;

using Sudoku.Android.ViewModels;
using Sudoku.Android.Views.Custom;

namespace Sudoku.Android.Views
{
	public class SudokuFragment : Fragment
	{
		private SudokuViewModel ViewModel { get; set; }

		private void ViewUpdated(Object sender, SudokuViewEventArgs e)
		{
			this.ViewModel.SetRowAndColumn(e.Row, e.Column);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			this.ViewModel = (SudokuViewModel) new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof (SudokuViewModel)));
			SudokuView view = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			view.SetRowAndColumn(this.ViewModel.Row, this.ViewModel.Column);
			view.Sudoku = this.ViewModel.Sudoku;
			this.ViewModel.Sudoku.Changed += new SudokuChangedEventHandler((sender, e) => view.Update());
			view.Changed += new SudokuViewEventHandler(this.ViewUpdated);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_sudoku, container, false);
	}
}