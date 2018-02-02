using System;
using System.Collections.Generic;
using System.Text;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
    interface IListService
    {
		List GetList( string listName );
		List GetCurrentList();
		List GetBasketList();
		void DeleteListItem( string listName, long id );
		void DeleteListItemFromCurrentList( long id );
		void DeleteListItemFromBasketList( long id );
		void AddItemToBasketList( long itemId, int quantity );
		void AddItemToCurrentList( long itemId, int quantity );
	}
}
