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
		private ControlFragment ControlFragment { get; set; }
		private SudokuViewModel ViewModel { get; set; }

		private void ViewUpdated(Object sender, SudokuViewEventArgs e)
		{
			this.ViewModel.SetRowAndColumn(e.Row, e.Column);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			this.ViewModel = (SudokuViewModel) new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof (SudokuViewModel)));
			SudokuView sudokuView = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			this.ControlFragment = (ControlFragment) this.ChildFragmentManager.FindFragmentById(Resource.Id.control_fragment);
			this.ControlFragment.State = this.ViewModel.State;
			sudokuView.SetRowAndColumn(this.ViewModel.Row, this.ViewModel.Column);
			sudokuView.Sudoku = this.ViewModel.Sudoku;
			this.ViewModel.Sudoku.Changed += new SudokuChangedEventHandler((sender, e) => sudokuView.Update());
			sudokuView.Changed += new SudokuViewEventHandler(this.ViewUpdated);
			this.ControlFragment.Changed += new ControlEventHandler((sender, e) => this.ViewModel.State = e);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_sudoku, container, false);
	}
}