using ShoppingList.Core.Model;
using ShoppingList.Core.Services;
using System.Linq;

namespace ShoppingList.Core.Controllers
{
	public class ShoppingController
    {
		/// <summary>
		/// An item in the current shopping list has been swiped.
		/// If the item was flung then move all instances of the item to the basket
		/// If the item was justed swiped then move a single item to the basket.
		/// This may have the same effect depending on the number of instances of the item (its quantity)
		/// </summary>
		/// <param name="item"></param>
		/// <param name="wasFlung"></param>
		public static int CurrentItemSwiped( ListItem item, bool wasFlung )
		{
			IListService service = new ListService();

			int quantityToMove = ( wasFlung == true ) ? ( int )item.Quantity : 1;

			// Remove the specified number of instances of the item from the current list and add to the basket 
			service.RemoveItemFromCurrentList( item.Id, quantityToMove );
			service.AddItemToBasketList( item.ItemId, quantityToMove );

			return quantityToMove;
		}

		/// <summary>
		/// An item in the basket list has been swiped.
		/// If the item was flung then move all instances of the item to the current list
		/// If the item was justed swiped then move a single item to the current list.
		/// This may have the same effect depending on the number of instances of the item (its quantity)
		/// </summary>
		/// <param name="item"></param>
		/// <param name="wasFlung"></param>
		public static int BasketItemSwiped( ListItem item, bool wasFlung )
		{
			IListService service = new ListService();

			int quantityToMove = ( wasFlung == true ) ? ( int )item.Quantity : 1;

			// Remove the specified number of instances from the basket list and add back to the current list
			service.RemoveItemFromBasketList( item.Id, quantityToMove );
			service.AddItemToCurrentList( item.ItemId, quantityToMove );

			return quantityToMove;
		}

		public static ListItem[] GetCurrentListItems()
		{
			return new ListService().GetCurrentList().ListItems.ToArray();
		}

		public static ListItem[] GetBasketListItems()
		{
			return new ListService().GetBasketList().ListItems.OrderByDescending( item => item.Id ).ToArray();
		}
	}
}
