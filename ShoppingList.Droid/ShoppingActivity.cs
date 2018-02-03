using Android.App;
using Android.OS;
using ShoppingList.Core.Services;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using System.Linq;
using Android.Widget;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	[Activity( Label = "Shopping List", MainLauncher = false )]
	public class ShoppingActivity: AppCompatActivity
	{
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			// Specify the target specific part of the database path
			new ConfigurationService().DatabasePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

			SetContentView( Resource.Layout.ShoppingScreen );

			SetSupportActionBar( FindViewById<Toolbar>( Resource.Id.toolbar ) );
			SupportActionBar.Title = ShoppingTitle;

			// We are now making a list
			GetSharedPreferences( IsShoppingStateString, FileCreationMode.Private ).Edit().PutBoolean( IsShoppingStateString, true ).Commit();

			// Get the current shopping list data and provide it to view responsible for displaying it
			itemsView = FindViewById<ListView>( Resource.Id.shoppingItems );
			adapter = new ListItemAdapter( this, new ListService().GetCurrentList().ListItems.ToArray() );
			itemsView.Adapter = adapter;

			// Do the same for the basket list
			basketView = FindViewById<ListView>( Resource.Id.basketItems );
			basketAdapter = new ListItemAdapter( this, new ListService().GetBasketList().ListItems.ToArray() );
			basketView.Adapter = basketAdapter;

			// Always start showing the current list - could save this
			basketView.Visibility = Android.Views.ViewStates.Gone;
			itemsView.TranslationX = 0;

			InitialiseShoppingTouchHandling();
			InitialiseBasketTouchHandling();
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( Resource.Menu.ShoppingMenu, menu );
			return base.OnCreateOptionsMenu( menu );
		}

		public override bool OnPrepareOptionsMenu( IMenu menu )
		{
			return base.OnPrepareOptionsMenu( menu );
		}

		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			if ( item.ItemId == Resource.Id.menuList )
			{
				StartActivity( new Intent( this, typeof( ListingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
			}

			return base.OnOptionsItemSelected( item );
		}

		/// <summary>
		/// Set up a touch handler and events for the shopping list
		/// </summary>
		private void InitialiseShoppingTouchHandling()
		{
			// Setup a touch listener for the current shopping list
			ListViewTouchListener shoppingListListener = new ListViewTouchListener( itemsView );
			itemsView.SetOnTouchListener( shoppingListListener );

			shoppingListListener.RightSwipeAllowed = true;
			shoppingListListener.RightGroupSwipe = false;

			shoppingListListener.LeftSwipeAllowed = true;
			shoppingListListener.LeftGroupSwipe = true;
			shoppingListListener.RightRevealView = basketView;

			// Hook into the FlingRight event
			shoppingListListener.FlingRightHandler += ( object sender, int position ) =>
			{
				// Move the selected item to the basket list
				if ( position != -1 )
				{
					// Get the ListItem from the adapter
					ListItem item = adapter[ position ];

					// Delete the item from the current list
					ListService service = new ListService();
					service.DeleteListItemFromCurrentList( item.Id );

					// Add the item to the basket list
					service.AddItemToBasketList( item.ItemId, 1 );

					// Refresh the data for both lists
					RefreshLists();
				}
			};

			// Hook into the FlingLeft event
			shoppingListListener.FlingLeftHandler += ( object sender, int position ) => 
			{
				// The basket should now be shown so update the title
				SupportActionBar.Title = BasketTitle;
			};
		}

		/// <summary>
		/// Set up a touch handler and events for the shopping list
		/// </summary>
		private void InitialiseBasketTouchHandling()
		{
			// Setup a touch listener for the basket list
			ListViewTouchListener basketListListener = new ListViewTouchListener( basketView );
			basketView.SetOnTouchListener( basketListListener );

			basketListListener.RightSwipeAllowed = true;
			basketListListener.RightGroupSwipe = true;
			basketListListener.LeftRevealView = itemsView;

			basketListListener.LeftSwipeAllowed = true;
			basketListListener.LeftGroupSwipe = false;

			basketListListener.FlingRightHandler += ( object sender, int position ) => 
			{
				// The shopping list should now be show so update the title
				SupportActionBar.Title = ShoppingTitle;
			};

			// Hook into the FlingLeft event
			basketListListener.FlingLeftHandler += ( object sender, int position ) => 
			{
				// Move the selected item back to the shopping list
				if ( position != -1 )
				{
					// Get the ListItem from the adapter
					ListItem item = basketAdapter[ position ];

					// Delete the item from the basket list
					ListService service = new ListService();
					service.DeleteListItemFromBasketList( item.Id );

					// Add the item to the current list
					service.AddItemToCurrentList( item.ItemId, 1 );

					// Refresh the data for both lists
					RefreshLists();
				}
			};
		}

		/// <summary>
		/// Refesh the shopping and basket lists from the data source
		/// </summary>
		private void RefreshLists()
		{
			adapter.Items = new ListService().GetCurrentList().ListItems.ToArray();
			adapter.NotifyDataSetChanged();
			basketAdapter.Items = new ListService().GetBasketList().ListItems.OrderByDescending( item => item.Id ).ToArray();
			basketAdapter.NotifyDataSetChanged();
		}

		/// <summary>
		/// The view displaying the current shopping list
		/// </summary>
		private ListView itemsView = null;

		/// <summary>
		/// The adapter providing the data to the view
		/// </summary>
		private ListItemAdapter adapter = null;

		/// <summary>
		/// The view displaying the shopping basket
		/// </summary>
		private ListView basketView = null;

		/// <summary>
		/// The adapter providing the data to the basket view
		/// </summary>
		private ListItemAdapter basketAdapter = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string ShoppingTitle = "Shopping";
		private const string BasketTitle = "Whats in your basket";

		private const string IsShoppingStateString = "isShopping";
	}
}

