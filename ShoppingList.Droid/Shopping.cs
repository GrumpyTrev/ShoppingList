using System;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using ShoppingList.Core.Model;
using ShoppingList.Core.Services;

namespace ShoppingList.Droid
{
	/// <summary>
	/// The Shopping class manages the views and user action involved in shopping mode
	/// </summary>
	class Shopping
	{
		/// <summary>
		/// Create a Shopping class for the specified activity
		/// </summary>
		/// <param name="activityContext">The activity owning this class</param>
		public Shopping( MainActivity activityContext )
		{
			activity = activityContext;

			// Get the current shopping list data and provide it to view responsible for displaying it
			itemsView = activity.FindViewById<ListView>( Resource.Id.shoppingItems );
			adapter = new ListItemAdapter( activity, new ListService().GetCurrentList().ListItems.ToArray() );
			itemsView.Adapter = adapter;

			// Do the same for the basket list
			basketView = activity.FindViewById<ListView>( Resource.Id.basketItems );
			basketAdapter = new ListItemAdapter( activity, new ListService().GetBasketList().ListItems.ToArray() );
			basketView.Adapter = basketAdapter;

			InitialiseShoppingTouchHandling();
			InitialiseBasketTouchHandling();

			// At start up we are currently assuming that the shopping list is being displayed, so
			// hide the basket. 
			// TODO This should be saved somewhere.
			basketView.Visibility = Android.Views.ViewStates.Gone;
		}

		public void SetActive()
		{
			activity.SupportActionBar.Title = ShoppingTitle;
			basketView.Visibility = Android.Views.ViewStates.Gone;
			itemsView.TranslationX = 0;
			RefreshLists();
		}

		public void OnPrepareOptionsMenu( IMenu menu, bool active )
		{
			savedMenu = menu;
		}

		public bool OnOptionsItemSelected( IMenuItem item )
		{
			return false;
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
				activity.SupportActionBar.Title = BasketTitle;
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
				activity.SupportActionBar.Title = ShoppingTitle;
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
		/// The parent MainActivity
		/// </summary>
		private MainActivity activity = null;

		private IMenu savedMenu = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string ShoppingTitle = "Shopping";
		private const string BasketTitle = "Whats in your basket";
	}
}