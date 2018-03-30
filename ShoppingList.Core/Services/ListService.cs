using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	internal class ListService: IListService
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

		public void DeleteListItem( string listName, long itemId, int quantity )
		{
			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				List targetList = context.Lists.Where( list => list.Name == listName )
					.Include( list => list.ListItems )
					.FirstOrDefault();

				if ( targetList != null )
				{
					ListItem itemToDelete = targetList.ListItems.Where( item => item.Id == itemId ).FirstOrDefault();

					if ( itemToDelete != null )
					{
						if ( itemToDelete.Quantity <= quantity )
						{
							context.ListItems.Remove( itemToDelete );
						}
						else
						{
							itemToDelete.Quantity -= quantity;
						}

						context.SaveChanges();
					}
				}
			}
		}

		public void RemoveItemFromCurrentList( long itemId, int quantity )
		{
			DeleteListItem( CurrentListName, itemId, quantity );
		}

		public void RemoveItemFromBasketList( long itemId, int quantity )
		{
			DeleteListItem( BasketListName, itemId, quantity );
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
					// If there is already an item with the same identity in the list then simply update its quantity
					ListItem itemToUpdate = targetList.ListItems.Where( item => item.ItemId == itemId ).FirstOrDefault();

					if ( itemToUpdate != null )
					{
						itemToUpdate.Quantity += quantity;
					}
					else
					{
						context.ListItems.Add( new ListItem { ItemId = itemId, ListId = targetList.Id, Quantity = quantity } );
					}

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
