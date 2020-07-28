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
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_sudoku, container, false);
	}
}