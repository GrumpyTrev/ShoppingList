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
		public Shopping( ShoppingActivity activityContext )
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
		/// The parent MainActivity
		/// </summary>
		private ShoppingActivity activity = null;

		private IMenu savedMenu = null;

	}
}