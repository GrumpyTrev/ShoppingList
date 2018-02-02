using Android.App;
using Android.OS;
using ShoppingList.Core.Services;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;

namespace ShoppingList.Droid
{
	[Activity( Label = "Shopping List", MainLauncher = true )]
	public class MainActivity: AppCompatActivity
	{
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			// Specify the target specific part of the database path
			new ConfigurationService().DatabasePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

			// Are we shopping or making a list
			preferences = GetPreferences( FileCreationMode.Private );
			isShopping = preferences.GetBoolean( IsShoppingStateString, true );

			if ( isShopping == true )
			{
				GoShopping();
			}
			else
			{
				LetsMakeAList();
			}
		}

		protected override void OnPause()
		{
			base.OnPause();

			ISharedPreferencesEditor editor = preferences.Edit();
			editor.PutBoolean( IsShoppingStateString, isShopping );
			editor.Commit();
		}

		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( Resource.Menu.MainMenu, menu );

			return base.OnCreateOptionsMenu( menu );
		}

		public override bool OnPrepareOptionsMenu( IMenu menu )
		{
			if ( isShopping == true )
			{
				menu.FindItem( Resource.Id.menuShop ).SetVisible( false );
				menu.FindItem( Resource.Id.menuList ).SetVisible( true );
				shoppingHandler.OnPrepareOptionsMenu( menu, true );
			}
			else
			{
				menu.FindItem( Resource.Id.menuShop ).SetVisible( true );
				menu.FindItem( Resource.Id.menuList ).SetVisible( false );
				listingHandler.OnPrepareOptionsMenu( menu, true );
			}

			return base.OnPrepareOptionsMenu( menu );
		}

		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			if ( item.ItemId == Resource.Id.menuList )
			{
				LetsMakeAList();
			}
			else if ( item.ItemId == Resource.Id.menuShop )
			{
				GoShopping();
			}
			else if ( shoppingHandler?.OnOptionsItemSelected( item ) == true )
			{
			}
			else if ( listingHandler?.OnOptionsItemSelected( item ) == true )
			{
			}
			
			return base.OnOptionsItemSelected( item );
		}

		private void GoShopping()
		{
			SetContentView( Resource.Layout.ShoppingScreen );

			SetSupportActionBar( FindViewById<Toolbar>( Resource.Id.toolbar ) );

			shoppingHandler = new Shopping( this );
			shoppingHandler.SetActive();

			isShopping = true;
		}

		private void LetsMakeAList()
		{
			SetContentView( Resource.Layout.ListingScreen );

			SetSupportActionBar( FindViewById<Toolbar>( Resource.Id.toolbar ) );

			listingHandler = new Listing( this );
			listingHandler.SetActive();

			isShopping = false;
		}

		private Shopping shoppingHandler = null;
		private Listing listingHandler = null;
		private ISharedPreferences preferences = null;

		private bool isShopping = true;

		private const string IsShoppingStateString = "isShopping";
	}
}

