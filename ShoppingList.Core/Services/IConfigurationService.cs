namespace ShoppingList.Core.Services
{
	interface IConfigurationService
    {
		string DatabasePath
		{
			set;
		}
    }
}
