using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	interface IListService
    {
		List GetList( string listName );
		List GetCurrentList();
		List GetBasketList();
		void DeleteListItem( string listName, long itemId, int quantity );
		void RemoveItemFromCurrentList( long itemId, int quantity );
		void RemoveItemFromBasketList( long itemId, int quantity );
		void AddItemToBasketList( long itemId, int quantity );
		void AddItemToCurrentList( long itemId, int quantity );
	}
}
