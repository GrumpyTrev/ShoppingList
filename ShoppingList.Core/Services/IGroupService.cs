using System.Collections.Generic;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	interface IGroupService
    {
		List<Group> GetGroups();
		List<object> GetGroupsAndItems();
		List<Item> GetItems();
	}
}
