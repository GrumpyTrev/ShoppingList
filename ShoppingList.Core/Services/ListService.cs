using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	public class ListService: IListService
	{
		public List GetList( string listName)
		{
			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				return context.Lists.Where( list => list.Name == listName )
					.Include( list => list.ListItems )
					.ThenInclude( itemInList => itemInList.Item )
					.ThenInclude( item => item.Group )
					.FirstOrDefault();
			}
		}

		public List GetCurrentList()
		{
			return GetList( CurrentListName );
		}

		public List GetBasketList()
		{
			return GetList( BasketListName );
		}

		public void DeleteListItem( string listName, long id )
		{
			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				List targetList = context.Lists.Where( list => list.Name == listName )
					.Include( list => list.ListItems )
					.FirstOrDefault();

				if ( targetList != null )
				{
					ListItem itemToDelete = targetList.ListItems.Where( item => item.Id == id ).FirstOrDefault();

					if ( itemToDelete != null )
					{
						context.ListItems.Remove( itemToDelete );
						context.SaveChanges();
					}
				}
			}
		}

		public void DeleteListItemFromCurrentList( long id )
		{
			DeleteListItem( CurrentListName, id );
		}

		public void DeleteListItemFromBasketList( long id )
		{
			DeleteListItem( BasketListName, id );
		}

		public void AddItemToList( string listName, long itemId, int quantity )
		{
			// Add a ListItem entry for the specified ID to the specified list
			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				List targetList = context.Lists.Where( list => list.Name == listName )
					.Include( list => list.ListItems )
					.FirstOrDefault();

				if ( targetList != null )
				{
					//targetList.ListItems.Add( new ListItem { ItemId = itemId, ListId = targetList.Id, Quantity = quantity } );
					context.ListItems.Add( new ListItem { ItemId = itemId, ListId = targetList.Id, Quantity = quantity } );
					context.SaveChanges();
				}
			}
		}

		public void AddItemToBasketList( long itemId, int quantity )
		{
			// Add a ListItem entry for the specified ID to the basket list
			AddItemToList( BasketListName, itemId, quantity );
		}

		public void AddItemToCurrentList( long itemId, int quantity )
		{
			// Add a ListItem entry for the specified ID to the basket list
			AddItemToList( CurrentListName, itemId, quantity );
		}

		private const string CurrentListName = "CurrentItems";
		private const string BasketListName = "BasketItems";
	}
}
