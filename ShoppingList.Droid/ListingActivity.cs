using Android.App;
using Android.OS;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Widget;
using ShoppingList.Core;
using ShoppingList.Core.Controllers;
using System;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	[Activity( Label = "Making A List", MainLauncher = false )]
	public class ListingActivity: AppCompatActivity
	{
		/// <summary>
		/// Called when the activity is first displayed
		/// Initialise the main view and the action bar and determine what should be displayed
		/// </summary>
		/// <param name="savedInstanceState"></param>
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			SetContentView( Resource.Layout.ListingScreen );

			// Initialise the action bar
			// TODO Wrap this up in a class
			Toolbar toolbar = FindViewById<Toolbar>( Resource.Id.toolbar );
			SetSupportActionBar( toolbar );
			SupportActionBar.SetDisplayShowTitleEnabled( false );
			toolbarTitle = toolbar.FindViewById< TextView >( Resource.Id.toolbar_title );
			toolbarSubtitle = toolbar.FindViewById<TextView>( Resource.Id.toolbar_subtitle );

			// Create a toast item to display feedback
			toast = Toast.MakeText( this, "", ToastLength.Short );

			// We are now making a list
			PersistentStorage.IsShopping = false;

			// Create the views that are going to display the data
			ListView availableItemsView = FindViewById<ListView>( Resource.Id.availableItems );
			ListView currentItemsView = FindViewById<ListView>( Resource.Id.shoppingItems );

			// Wrap up these views to add touch handling
			currentList = new CurrentListWrapper( this, currentItemsView, availableItemsView, CurrentListTitle );

			availableList = new AvailableListWrapper( this, availableItemsView, currentItemsView, AvailableItemsTitle,
				new ItemSort( new ItemSort.SortState[] { ItemSort.SortState.Grouped, ItemSort.SortState.Alphabetic,
					ItemSort.SortState.Unsorted }, ItemSort.SortState.Grouped, AvailableItemsTitle ) );

			// Hook into the available items view being shown event
			currentList.RevealActive += ( object sender, EventArgs args ) => 
			{
				toolbarTitle.Text = availableList.ToolbarTitle;
				toolbarSubtitle.Text = "";
			};

			// Handle the swiping of a current item
			currentList.ItemSwiped += ( object sender, ListViewWrapper<ListItem>.SwipeItemEventArgs<ListItem> args ) =>
			{
				int itemsMoved = ListingController.CurrentItemSwiped( args.Item, args.WasFlung );

				if ( itemsMoved > 1 )
				{
					toast.SetText( string.Format( "{0} {1} removed from list", itemsMoved, args.Item.Item.Name ) );
				}
				else
				{
					toast.SetText( string.Format( "{0} removed from list", args.Item.Item.Name ) );
				}

				toast.Show();

				currentList.DataSetChanged();
			};

			// Hook into the current items view being shown event
			availableList.RevealActive += ( object sender, EventArgs args ) => 
			{
				// SupportActionBar.Title = currentList.ToolbarTitle;
				toolbarTitle.Text = currentList.ToolbarTitle;
				toolbarSubtitle.Text = "unnamed list";
			};

			// Handle the swiping of an available item
			availableList.ItemSwiped += ( object sender, ListViewWrapper<object>.SwipeItemEventArgs<object> args ) =>
			{
				Item itemAdded = ( Item )args.Item;

				int itemsMoved = ListingController.AvailableItemSwiped( itemAdded, args.WasFlung );

				if ( itemsMoved > 1 )
				{
					toast.SetText( string.Format( "{0} {1} added to list", itemsMoved, itemAdded.Name ) );
				}
				else
				{
					toast.SetText( string.Format( "{0} added to list", itemAdded.Name ) );
				}

				toast.Show();

				currentList.DataSetChanged();
			};

//			currentItemsView.LongClickable = true;
			currentItemsView.ItemLongClick += ( object sender, AdapterView.ItemLongClickEventArgs args ) =>
			{
				toast.SetText( "Current items long clicked" );
				toast.Show();
			};

			// Always start showing the available items list
			currentItemsView.Visibility = Android.Views.ViewStates.Gone;
			availableItemsView.TranslationX = 0;
			// SupportActionBar.Title = availableList.ToolbarTitle;
			toolbarTitle.Text = availableList.ToolbarTitle;
		}

		private void CurrentItemsView_LongClick( object sender, View.LongClickEventArgs e )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the activity is first displayed
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			// Initialise this activity's custom menu
			MenuInflater.Inflate( Resource.Menu.ListingMenu, menu );

			return base.OnCreateOptionsMenu( menu );
		}

		/// <summary>
		/// Called just before the menu is displayed.
		/// Save the menu for later enable/disable actions
		/// Show or hide the items in the menu according to the current sort mode
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public override bool OnPrepareOptionsMenu( IMenu menu )
		{
			savedMenu = menu;

			ShowOrHideMenuItems();

			return base.OnPrepareOptionsMenu( menu );
		}

		/// <summary>
		/// Called when a menu item has been selected
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			if ( item != null )
			{
				if ( item.ItemId == Resource.Id.menuShop )
				{
					// Start the shopping activity
					StartActivity( new Intent( this, typeof( ShoppingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
					Finish();
				}
				else if ( ( item.ItemId == Resource.Id.menuSortItems ) || ( item.ItemId == Resource.Id.menuSortGroups ) ||
					( item.ItemId == Resource.Id.menuSortNoSort ) )
				{
					availableList.SortOrderHandler.SetNext();

					ShowOrHideMenuItems();

					availableList.DataSetChanged();
				}
			}
			return base.OnOptionsItemSelected( item );
		}

		/// <summary>
		/// Show or hide the menu items according to the current sort option
		/// </summary>
		private void ShowOrHideMenuItems()
		{
			savedMenu.FindItem( Resource.Id.menuSortGroups ).SetVisible( availableList.SortOrderHandler.Next == ItemSort.SortState.Grouped );
			savedMenu.FindItem( Resource.Id.menuSortItems ).SetVisible( availableList.SortOrderHandler.Next == ItemSort.SortState.Alphabetic );
			savedMenu.FindItem( Resource.Id.menuSortNoSort ).SetVisible( availableList.SortOrderHandler.Next == ItemSort.SortState.Unsorted );
		}

		/// <summary>
		/// The view displaying the group item list
		/// </summary>
		private AvailableListWrapper availableList = null;

		/// <summary>
		/// The view displaying the current shopping list
		/// </summary>
		private CurrentListWrapper currentList = null;

		private TextView toolbarTitle = null;
		private TextView toolbarSubtitle = null;

		/// <summary>
		/// The custom menu
		/// </summary>
		private IMenu savedMenu = null;

		/// <summary>
		/// Feedback for moving items to and from the list
		/// </summary>
		private Toast toast = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string CurrentListTitle = "Your current list";
		private const string AvailableItemsTitle = "Available items";
	}
}

