using Android.App;
using Android.OS;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Widget;
using ShoppingList.Core;
using ShoppingList.Core.Controllers;
using ShoppingList.Core.Model;
using System;

namespace ShoppingList.Droid
{
	[Activity( Label = "Shopping List", MainLauncher = false )]
	public class ShoppingActivity: AppCompatActivity
	{
		/// <summary>
		/// Called when the activity is first displayed
		/// Initialise the main view and the action bar and determine what should be displayed
		/// </summary>
		/// <param name="savedInstanceState"></param>
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate( savedInstanceState );

			SetContentView( Resource.Layout.ShoppingScreen );

			// Initialise the action bar 
			SetSupportActionBar( FindViewById<Toolbar>( Resource.Id.toolbar ) );

			// Create a toast item to display feedback
			toast = Toast.MakeText( this, "", ToastLength.Short );

			// We are now making a list
			PersistentStorage.IsShopping = true;

			// Create the views that are going to display the data
			ListView currentItemsView = FindViewById<ListView>( Resource.Id.shoppingItems );
			ListView basketItemsView = FindViewById<ListView>( Resource.Id.basketItems );

			// Wrap up these views to add touch handling
			currentList = new CurrentListWrapper( this, currentItemsView, basketItemsView, CurrentListTitle );
			basketList = new ShoppingBasketListWrapper( this, basketItemsView, currentItemsView, BasketListTitle );

			// Hook into the basket view being shown event
			currentList.RevealActive += ( object sender, EventArgs args ) => 
			{
				SupportActionBar.Title = basketList.ToolbarTitle;
			};

			// Handle the swiping of a current item
			currentList.ItemSwiped += ( object sender, ListViewWrapper<ListItem>.SwipeItemEventArgs< ListItem > args ) =>
			{
				int itemsMoved = ShoppingController.CurrentItemSwiped( args.Item, args.WasFlung );
				if ( itemsMoved > 1 )
				{
					toast.SetText( string.Format( "{0} {1} put in basket", itemsMoved, args.Item.Item.Name ) );
				}
				else
				{
					toast.SetText( string.Format( "{0} put in basket", args.Item.Item.Name ) );
				}

				toast.Show();

				currentList.DataSetChanged();
				basketList.DataSetChanged();
			};

			// Hook into the current list view being shown event
			basketList.RevealActive += ( object sender, EventArgs args ) => 
			{
				SupportActionBar.Title = currentList.ToolbarTitle;
			};

			// Handle the swiping of a basket item
			basketList.ItemSwiped += ( object sender, ListViewWrapper<ListItem>.SwipeItemEventArgs<ListItem> args ) =>
			{
				int itemsMoved = ShoppingController.BasketItemSwiped( args.Item, args.WasFlung );

				if ( itemsMoved > 1 )
				{
					toast.SetText( string.Format( "{0} {1} removed from basket", itemsMoved, args.Item.Item.Name ) );
				}
				else
				{
					toast.SetText( string.Format( "{0} removed from basket", args.Item.Item.Name ) );
				}

				toast.Show();

				currentList.DataSetChanged();
				basketList.DataSetChanged();
			};

			// Always start showing the current list
			basketItemsView.Visibility = Android.Views.ViewStates.Gone;
			currentItemsView.TranslationX = 0;
			SupportActionBar.Title = currentList.ToolbarTitle;
		}

		/// <summary>
		/// Called when the activity is first displayed
		/// </summary>
		/// <param name="menu"></param>
		/// <returns></returns>
		public override bool OnCreateOptionsMenu( IMenu menu )
		{
			MenuInflater.Inflate( Resource.Menu.ShoppingMenu, menu );
			return base.OnCreateOptionsMenu( menu );
		}

		/// <summary>
		/// Called when a menu item has been selected
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool OnOptionsItemSelected( IMenuItem item )
		{
			if ( item.ItemId == Resource.Id.menuList )
			{
				// Start the listing activity
				StartActivity( new Intent( this, typeof( ListingActivity ) ).AddFlags( ActivityFlags.NoHistory ) );
				Finish();
			}

			return base.OnOptionsItemSelected( item );
		}

		/// <summary>
		/// The view displaying the current shopping list
		/// </summary>
		private CurrentListWrapper currentList = null;

		/// <summary>
		/// The view displaying the shopping basket
		/// </summary>
		private ShoppingBasketListWrapper basketList = null;

		/// <summary>
		/// Feedback for moving items to and from the basket
		/// </summary>
		private Toast toast = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string BasketListTitle = "Whats in your basket";
		private const string CurrentListTitle = "Shopping";
	}
}

