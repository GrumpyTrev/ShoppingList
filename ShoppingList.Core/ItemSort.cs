using System;

namespace ShoppingList.Core
{
	public class ItemSort
    {
		public enum SortState { Unsorted, Alphabetic, Grouped };

		public ItemSort( SortState[] order, SortState defaultState, string storageName )
		{
			// Store the order and get the current state from persistent storage
			SortOrder = order;

			StorageName = storageName + "sortOrder";

			CurrentOrder = ( SortState )Enum.Parse( typeof( SortState ), PersistentStorage.GetStringItem( StorageName, defaultState.ToString() ) );

			// if this order is not in the allowed states then set the current order to the first item in the states
			if ( Array.Exists( SortOrder, element => ( element == CurrentOrder ) ) == false )
			{
				CurrentOrder = SortOrder[ 0 ];
			}
		}

		public SortState Next
		{ 
			get
			{
				return SortOrder[ NextIndex() ];
			}
		}

		public void SetNext()
		{
			CurrentOrder = SortOrder[ NextIndex() ];
			PersistentStorage.SetStringItem( StorageName, CurrentOrder.ToString() );
		}

		private int NextIndex()
		{
			int index = Array.FindIndex( SortOrder, element => ( element == CurrentOrder ) );
			if ( ( index < 0 ) || ( index == ( SortOrder.Length - 1 ) ) )
			{
				index = 0;
			}
			else
			{
				index++;
			}

			return index;
		}

		public SortState CurrentOrder { get; private set; }

		private SortState[] SortOrder { get; set; }

		private string StorageName { get; set; }
    }
}
