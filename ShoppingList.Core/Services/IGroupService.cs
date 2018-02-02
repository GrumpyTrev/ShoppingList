using System;
using System.Collections.Generic;
using System.Text;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
    interface IGroupService
    {
		List<Group> GetGroups();
		List<object> GetGroupsAndItems();
    }
}
