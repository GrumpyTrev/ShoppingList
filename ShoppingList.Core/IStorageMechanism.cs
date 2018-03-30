﻿namespace ShoppingList.Core
{
	/// <summary>
	/// The IStorageMechanism specifies the storage methods that must be supported by the target hardware
	/// </summary>
	public interface IStorageMechanism
    {
		/// <summary>
		/// Get the value of the specified boolean item
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		bool GetBoolItem( string itemName, bool defaultValue );

		/// <summary>
		/// Set the value of the specified boolean item
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="value"></param>
		void SetBoolItem( string itemName, bool value );
    }
}
