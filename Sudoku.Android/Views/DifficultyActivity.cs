using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

using AndroidX.AppCompat.App;

using Xamarin.Essentials;

namespace Sudoku.Android.Views
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
	public class DifficultyActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Platform.Init(this, savedInstanceState);
			// Set our view from the "main" layout resource
			this.SetContentView(Resource.Layout.activity_difficulty);
		}

		public override void OnRequestPermissionsResult(int requestCode, String[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}