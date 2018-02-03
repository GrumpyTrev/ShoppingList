using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ShoppingList.Core.Services;

namespace ShoppingList.Droid
{
	class Listing
	{
		/// <summary>
		/// Create a Shopping class for the specified activity
		/// </summary>
		/// <param name="activityContext">The activity owning this class</param>
		public Listing( ShoppingActivity activityContext )
		{
			activity = activityContext;

		}

		public void SetActive()
		{
			activity.SupportActionBar.Title = ListingTitle;
			RefreshLists();
		}

		public void OnPrepareOptionsMenu( IMenu menu, bool active )
		{
			savedMenu = menu;

			if ( active == true )
			{
				menu.FindItem( Resource.Id.menuSortItems ).SetVisible( true );
				menu.FindItem( Resource.Id.menuSortGroups ).SetVisible( false );

			}
			else
			{
				menu.FindItem( Resource.Id.menuSortItems ).SetVisible( false );
				menu.FindItem( Resource.Id.menuSortGroups ).SetVisible( false );
			}
		}

		public bool OnOptionsItemSelected( IMenuItem item )
		{
			bool handled = false;

			if ( item.ItemId == Resource.Id.menuSortItems )
			{
				item.SetVisible( false );
				savedMenu.FindItem( Resource.Id.menuSortGroups ).SetVisible( true );
				handled = true;
			}
			else if ( item.ItemId == Resource.Id.menuSortGroups )
			{
				item.SetVisible( false );
				savedMenu.FindItem( Resource.Id.menuSortItems ).SetVisible( true );
				handled = true;
			}

			return handled;
		}

		/// <summary>
		/// Refesh the shopping and basket lists from the data source
		/// </summary>
		private void RefreshLists()
		{
			adapter.Items = new GroupService().GetGroupsAndItems().ToArray();
			adapter.NotifyDataSetChanged();
		}

		/// <summary>
		/// The parent MainActivity
		/// </summary>
		private ShoppingActivity activity = null;

		/// <summary>
		/// Toolbar titles
		/// </summary>
		private const string ListingTitle = "Making a list";

		private IMenu savedMenu = null;

	}
}