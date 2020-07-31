using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Icu.Text;
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
		private ControlEventHandler ControlFragmentChanged { get; set; }
		private EventHandler SingleButtonFragmentClicked { get; set; }
		private SudokuViewEventHandler SudokuViewChanged { get; set; }
		private SudokuViewModel ViewModel { get; set; }
		private EventHandler ViewModelCompleted { get; set; }
		private EventHandler ViewModelSudokuChanged { get; set; }
		private ControlEventHandler ViewModelStateChangedControlFragmentUpdate { get; set; }
		private ControlEventHandler ViewModelStateChangedSudokuViewUpdate { get; set; }

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

		/*public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);*/
		private void Start()
		{
			this.ViewModel = (SudokuViewModel) new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof (SudokuViewModel)));
			ActionType action = this.ViewModel.Action = (ActionType) this.Activity.Intent.Extras.GetInt("action", 1);

			switch (action)
			{
				case ActionType.Continue:
					this.ViewModel.OpenAsync(this.Context.FilesDir, SudokuDifficulty.None);
					break;
				case ActionType.Generate:
				case ActionType.New:
					SudokuDifficulty difficulty = (SudokuDifficulty) this.Activity.Intent.Extras.GetInt("difficulty", 1);
					this.ViewModel.OpenAsync(this.Context.FilesDir, difficulty);
					break;
				case ActionType.Play:
					break;
				default:
					throw new NotSupportedException();
			}

			SudokuView sudokuView = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			ControlFragment controlFragment;
			SingleButtonFragment singleButtonFragment;
			this.SudokuViewChanged = new SudokuViewEventHandler((sender, e) => this.ViewModel.SetRowAndColumn(e.Row, e.Column));
			this.ViewModelCompleted = new EventHandler(this.OnCompletion);

			this.ViewModelStateChangedSudokuViewUpdate = new ControlEventHandler((sender, e) =>
			{
				sudokuView.SelectedNumber = e.Number;
				sudokuView.SetRowAndColumn(this.ViewModel.Row, this.ViewModel.Column);
			});

			this.ViewModelSudokuChanged = new EventHandler((sender, e) =>
			{
				sudokuView.Sudoku = this.ViewModel.Sudoku;
				sudokuView.Update();
			});

			sudokuView.Changed += this.SudokuViewChanged;
			this.ViewModel.StateChanged += this.ViewModelStateChangedControlFragmentUpdate;
			this.ViewModel.StateChanged += this.ViewModelStateChangedSudokuViewUpdate;
			this.ViewModel.Completed += this.ViewModelCompleted;
			this.ViewModel.SudokuChanged += this.ViewModelSudokuChanged;

			sudokuView.Sudoku = this.ViewModel.Sudoku;
			sudokuView.SetRowAndColumn(this.ViewModel.Row, this.ViewModel.Column);
			sudokuView.SelectedNumber = this.ViewModel.State.Number;
			FragmentTransaction ft = this.Activity.SupportFragmentManager.BeginTransaction(); ;

			switch (action)
			{
				case ActionType.Continue:
				case ActionType.New:
				case ActionType.Play:
					//controlFragment = this.ChildFragmentManager.FindFragmentById(Resource.Id.control_fragment) as ControlFragment;
					controlFragment = new ControlFragment();
					this.ControlFragmentChanged = new ControlEventHandler((sender, e) => this.ViewModel.State = e);
					this.ViewModelStateChangedControlFragmentUpdate = new ControlEventHandler((sender, e) => controlFragment.State = e);
					controlFragment.Changed += this.ControlFragmentChanged;
					controlFragment.State = this.ViewModel.State;
					ft.Add(Resource.Id.fragment_frame_layout, controlFragment, "control_fragment");
					ft.Commit();
					break;

				case ActionType.Generate:
					if (this.ViewModel.ButtonText is null)
					{
						this.ViewModel.ButtonText = this.Resources.GetString(Resource.String.add_button_text);
					}

					singleButtonFragment = new SingleButtonFragment(this.ViewModel.ButtonText);
					this.SingleButtonFragmentClicked = new EventHandler(this.OnSingleButtonFragmentClick);
					singleButtonFragment.Click += this.SingleButtonFragmentClicked;
					ft.Add(Resource.Id.fragment_frame_layout, singleButtonFragment, "button_fragment");
					ft.Commit();
					break;
			}
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

		public override void OnDestroy()
		{
			this.ViewModel.SaveAsync(this.Context.FilesDir);
			base.OnDestroy();
		}

		//public override void OnDestroyView()
		private void Stop()
		{
			SudokuView sudokuView = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			ControlFragment controlFragment;
			SingleButtonFragment singleButtonFragment;

			switch (this.ViewModel.Action)
			{
				case ActionType.Continue:
				case ActionType.New:
				case ActionType.Play:
					controlFragment = this.FragmentManager.FindFragmentByTag("control_fragment") as ControlFragment; /// this.ChildFragmentManager.FindFragmentById(Resource.Id.control_fragment) as ControlFragment;
					controlFragment.Changed -= this.ControlFragmentChanged;
					break;
				case ActionType.Generate:
					singleButtonFragment = this.FragmentManager.FindFragmentByTag("button_fragment") as SingleButtonFragment;
					singleButtonFragment.Click -= this.SingleButtonFragmentClicked;
					break;
			}

			sudokuView.Changed -= this.SudokuViewChanged;
			this.ViewModel.StateChanged -= this.ViewModelStateChangedControlFragmentUpdate;
			this.ViewModel.StateChanged -= this.ViewModelStateChangedSudokuViewUpdate;
			this.ViewModel.Completed -= this.ViewModelCompleted;
			this.ViewModel.SudokuChanged -= this.ViewModelSudokuChanged;
		}

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

		public override void OnStart()
		{
			base.OnStart();
			this.Start();
			this.ViewModel.StartAsync(this.Context.FilesDir);
		}

		public override void OnStop()
		{
			this.ViewModel.StopAsync(this.Context.FilesDir);
			this.Stop();
			base.OnStop();
		}

		private void OnSingleButtonFragmentClick(Object sender, EventArgs e)
		{
			SingleButtonFragment singleButtonFragment = this.FragmentManager.FindFragmentByTag("button_fragment") as SingleButtonFragment;
			SudokuView sudokuView = this.View.FindViewById<SudokuView>(Resource.Id.sudoku_view);
			String addText = this.Resources.GetString(Resource.String.add_button_text);
			String removeText = this.Resources.GetString(Resource.String.remove_button_text);
			String playText = this.Resources.GetString(Resource.String.play_button_text);

			if (singleButtonFragment.Text == addText)
			{
				SudokuGenerator.AddNumbers(this.ViewModel.Sudoku);
				sudokuView.Update();
				this.ViewModel.ButtonText = singleButtonFragment.Text = removeText;
			}

			else if (singleButtonFragment.Text == removeText)
			{
				SudokuGenerator.RemoveNumbers(this.ViewModel.Sudoku);
				sudokuView.Update();
				this.ViewModel.ButtonText = singleButtonFragment.Text = playText;
			}

			else if (singleButtonFragment.Text == playText)
			{
				this.Stop();
				FragmentTransaction ft = this.FragmentManager.BeginTransaction();
				ft.Remove(singleButtonFragment);
				ft.Commit();
				this.Activity.Intent.PutExtra("action", (int) ActionType.Play);
				this.Activity.Intent.Extras.PutInt("action", (int) ActionType.Play);
				ActionType action = (ActionType) this.Activity.Intent.Extras.GetInt("action", (int) ActionType.None);
				this.ViewModel.ButtonText = null;
				this.Start();
			}
		}
	}
}