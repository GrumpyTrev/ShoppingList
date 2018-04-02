using ShoppingList.Core.Model;
using ShoppingList.Core.Services;

namespace ShoppingList.Core.Controllers
{
	public class ListingController
    {
		/// <summary>
		/// An item in the current shopping list has been swiped.
		/// If the item was flung then delete all instances of the item
		/// If the item was justed swiped then delete a single item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="wasFlung"></param>
		public static int CurrentItemSwiped( ListItem item, bool wasFlung )
		{
			int quantityToMove = ( wasFlung == true ) ? ( int )item.Quantity : 1;

			new ListService().RemoveItemFromCurrentList( item.Id, quantityToMove );

			return quantityToMove;
		}

		/// <summary>
		/// An item in the available list has been swiped.
		/// Add an instance of the item to the current list.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="wasFlung"></param>
		public static int AvailableItemSwiped( Item item, bool wasFlung )
		{
			int quantityToMove = 1;

			new ListService().AddItemToCurrentList( item.Id, quantityToMove );

			return quantityToMove;
		}

		public static Item[] GetItems( bool ordered )
		{
			return new GroupService().GetItems( ordered ).ToArray();
		}

		public static object[] GetGroupsAndItems()
		{
			return new GroupService().GetGroupsAndItems().ToArray();
		}
	}
}
