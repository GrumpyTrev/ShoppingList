using Android.Content;
using ShoppingList.Core;

namespace ShoppingList.Droid
{
	class StorageMechanism : IStorageMechanism
	{
		public StorageMechanism( Context context ) => StorageContext = context;

		/// <summary>
		/// Get a boolean item from the storage
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public bool GetBoolItem( string itemName, bool defaultValue ) =>
			StorageContext.GetSharedPreferences( StorageName, FileCreationMode.Private ).GetBoolean( itemName, defaultValue );

		/// <summary>
		/// Set a boolean item in the storage
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="value"></param>
		public void SetBoolItem( string itemName, bool value ) =>
			StorageContext.GetSharedPreferences( StorageName, FileCreationMode.Private ).Edit().PutBoolean( itemName, value ).Commit();

		/// <summary>
		/// Get the value of the specified string item
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetStringItem( string itemName, string defaultValue ) =>
			StorageContext.GetSharedPreferences( StorageName, FileCreationMode.Private ).GetString( itemName, defaultValue );

		/// <summary>
		/// Set the value of the specified string item
		/// </summary>
		/// <param name="itemName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public void SetStringItem( string itemName, string value ) =>
			StorageContext.GetSharedPreferences( StorageName, FileCreationMode.Private ).Edit().PutString( itemName, value ).Commit();

		/// <summary>
		/// The Context object to access the storage object
		/// </summary>
		private static Context StorageContext { get; set; } = null;

		/// <summary>
		/// Storage names
		/// </summary>
		private const string StorageName = "LetsGoShoppingStrorage";
	}
}