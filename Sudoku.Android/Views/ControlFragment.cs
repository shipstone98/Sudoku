using Android.OS;
using Android.Views;

using AndroidX.Fragment.App;

namespace Sudoku.Android.Views
{
	public class ControlFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_control, container, false);
	}
}