using Android.App;
using Android.OS;
using ShoppingList.Core.Services;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Widget;

namespace ShoppingList.Droid
{
	[Activity( Label = "Making A List", MainLauncher = false )]
	public class ListingActivity: AppCompatActivity
	{
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			// Specify the target specific part of the database path
			new ConfigurationService().DatabasePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

			SetContentView( Resource.Layout.ListingScreen );

			SetSupportActionBar( FindViewById<Toolbar>( Resource.Id.toolbar ) );
			SupportActionBar.Title = ListingTitle;

			// We are now making a list
			GetSharedPreferences( IsShoppingStateString, FileCreationMode.Private ).Edit().PutBoolean( IsShoppingStateString, false ).Commit();

			// Get the current group item data and provide it to view responsible for displaying it
			itemsView = FindViewById<ListView>( Resource.Id.availableItems );
			adapter = new GroupItemAdapter( this, new GroupService().GetGroupsAndItems().ToArray() );
			itemsView.Adapter = adapter;
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( Resource.Menu.ListingMenu, menu );
			return base.OnCreateOptionsMenu( menu );
		}

		public override bool OnPrepareOptionsMenu( IMenu menu )
		{
			savedMenu = menu;

			menu.FindItem( Resource.Id.menuSortItems ).SetVisible( true );
			menu.FindItem( Resource.Id.menuSortGroups ).SetVisible( false );

			return base.OnPrepareOptionsMenu( menu );
		}

		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			if ( item.ItemId == Resource.Id.menuShop )
			{
				StartActivity( new Intent( this, typeof( ShoppingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
			}
			if ( item.ItemId == Resource.Id.menuSortItems )
			{
				item.SetVisible( false );
				savedMenu.FindItem( Resource.Id.menuSortGroups ).SetVisible( true );
			}
			else if ( item.ItemId == Resource.Id.menuSortGroups )
			{
				item.SetVisible( false );
				savedMenu.FindItem( Resource.Id.menuSortItems ).SetVisible( true );
			}

			return base.OnOptionsItemSelected( item );
		}

		/// <summary>
		/// The view displaying the group item list
		/// </summary>
		private ListView itemsView = null;

		/// <summary>
		/// The adapter providing the data to the view
		/// </summary>
		private GroupItemAdapter adapter = null;

		private IMenu savedMenu = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string ListingTitle = "Making a list";

		private const string IsShoppingStateString = "isShopping";
	}
}

