using Android.OS;
using Android.Views;

using AndroidX.Fragment.App;

using Sudoku.Android.Views.Custom;

namespace Sudoku.Android.Views
{
	public class SudokuFragment : Fragment
	{
		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Sudoku sudoku = new Sudoku(9, SudokuDifficulty.Easy); //Change this to get details from DifficultyActivity
			SudokuView view = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			view.Sudoku = sudoku;
			//sudoku.Changed += new SudokuChangedEventHandler((sender, e) => view.SetRowAndColumn(e.Row, e.Column));
			sudoku.Changed += new SudokuChangedEventHandler((sender, e) => view.Update());
			SudokuGenerator.AddNumbers(sudoku);
			SudokuGenerator.RemoveNumbers(sudoku);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_sudoku, container, false);
	}
}