using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using Xamarin.Essentials;

namespace Sudoku.Android.Views
{
	[Activity(Label = "AboutActivity")]
	public class AboutActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Platform.Init(this, savedInstanceState);
			// Set our view from the "main" layout resource
			this.SetContentView(Resource.Layout.activity_about);
		}

		public override void OnRequestPermissionsResult(int requestCode, String[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}