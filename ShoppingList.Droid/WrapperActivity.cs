using Android.App;
using Android.OS;
using Android.Content;
using ShoppingList.Core;

namespace ShoppingList.Droid
{
	/// <summary>
	/// Main activity used to launch either the shopping of listing activities
	/// </summary>
	[Activity( Label = "Lets Go Shopping", MainLauncher = true )]
	public class WrapperActivity: Activity
	{
		/// <summary>
		/// Launch the correct activity
		/// </summary>
		/// <param name="savedInstanceState"></param>
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			// Specify the target specific part of the database path
			new Core.Services.ConfigurationService().DatabasePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

			// Create a StorageMechanism instance and pass up to the core
			PersistentStorage.StorageMechanism = new StorageMechanism( this );

			// Start either the shopping or listing activities according to the IsShopping persisted flag
			StartActivity( new Intent( this, ( PersistentStorage.IsShopping == true ) ? typeof( ShoppingActivity ) : typeof( ListingActivity )  )
				.AddFlags( ActivityFlags.NoHistory ) );

			// Exit this activity
			Finish();
		}
	}
}

