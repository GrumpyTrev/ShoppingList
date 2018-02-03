using System.IO;

namespace ShoppingList.Core.Services
{
	public class ConfigurationService: IConfigurationService
	{
		public string DatabasePath
		{
			set
			{
				databasePath = Path.Combine( value, "ShoppingList.db3" );
			}
		}

		static public string FullDatabasePath
		{
			get
			{
				return databasePath;
			}
		}

		/// <summary>
		/// Static pathname for the database formed from target specific path and fixed filename
		/// </summary>
		static private string databasePath = "";
	}
}
