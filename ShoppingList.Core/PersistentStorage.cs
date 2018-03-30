
namespace ShoppingList.Core
{
	public class PersistentStorage
	{
		public static bool IsShopping
		{
			get
			{
				if ( isShopping == null )
				{
					isShopping = StorageMechanism.GetBoolItem( IsShoppingStateString, true );
				}

				return isShopping.Value;
			}

			set
			{
				isShopping = value;
				StorageMechanism.SetBoolItem( IsShoppingStateString, isShopping.Value );
			}
		}

		public static bool IsSortedByGroup
		{
			get
			{
				if ( isSortedByGroup == null )
				{
					isSortedByGroup = StorageMechanism.GetBoolItem( IsSortedByGroupString, true );
				}

				return isSortedByGroup.Value;
			}

			set
			{
				isSortedByGroup = value;
				StorageMechanism.SetBoolItem( IsSortedByGroupString, isSortedByGroup.Value );
			}
		}

		public static IStorageMechanism StorageMechanism
		{
			private get;
			set;
		} 
		= null;

		/// <summary>
		/// Local copy of the IsShopping persisted value
		/// </summary>
		private static bool? isShopping = null;

		/// <summary>
		/// Local copy of the IsSortedByGroup persisted value
		/// </summary>
		private static bool? isSortedByGroup = null;

		/// <summary>
		/// Storage names
		/// </summary>
		private const string IsShoppingStateString = "IsShopping";
		private const string IsSortedByGroupString = "isSortedByGroup";
	}
}