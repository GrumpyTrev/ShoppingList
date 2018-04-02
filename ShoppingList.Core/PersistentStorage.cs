
using System.Collections.Generic;

namespace ShoppingList.Core
{
	public class PersistentStorage
	{
		public static bool GetBoolItem( string itemName, bool defaultState )
		{
			if ( cachedItems.ContainsKey( itemName ) == false )
			{
				cachedItems[ itemName ] = StorageMechanism.GetBoolItem( itemName, defaultState );
			}

			return ( bool )cachedItems[ itemName ];
		}

		public static void SetBoolItem( string itemName, bool state )
		{
			cachedItems[ itemName ] = state;
			StorageMechanism.SetBoolItem( itemName, state );
		}

		public static string GetStringItem( string itemName, string defaultState )
		{
			if ( cachedItems.ContainsKey( itemName ) == false )
			{
				cachedItems[ itemName ] = StorageMechanism.GetStringItem( itemName, defaultState );
			}

			return ( string )cachedItems[ itemName ];
		}

		public static void SetStringItem( string itemName, string state )
		{
			cachedItems[ itemName ] = state;
			StorageMechanism.SetStringItem( itemName, state );
		}

		public static bool IsShopping
		{
			get
			{
				return GetBoolItem( IsShoppingStateString, true );
			}

			set
			{
				SetBoolItem( IsShoppingStateString, value );
			}
		}

		public static IStorageMechanism StorageMechanism
		{
			private get;
			set;
		} 
		= null;

		/// <summary>
		/// Some items that have already been retrived from persistent storage
		/// </summary>
		private static Dictionary<string, object> cachedItems = new Dictionary<string, object>();

		/// <summary>
		/// Storage names
		/// </summary>
		private const string IsShoppingStateString = "IsShopping";
	}
}