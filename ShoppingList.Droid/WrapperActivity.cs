using Android.App;
using Android.OS;
using Android.Content;

namespace ShoppingList.Droid
{
	[Activity( Label = "Lets Go Shopping", MainLauncher = true )]
	public class WrapperActivity: Activity
	{
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			// Are we shopping or making a list
			if ( GetSharedPreferences( IsShoppingStateString, FileCreationMode.Private ).GetBoolean( IsShoppingStateString, true ) == true )
			{
				StartActivity( new Intent( this, typeof( ShoppingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
			}
			else
			{
				StartActivity( new Intent( this, typeof( ListingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
			}
			Finish();
		}

		private const string IsShoppingStateString = "isShopping";
	}
}

