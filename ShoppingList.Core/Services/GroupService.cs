using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	public class GroupService: IGroupService
	{
		public List<Group> GetGroups()
		{
			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				return context.Groups.ToList();
			}
		}

		public List<object> GetGroupsAndItems()
		{
			List<object> returnedList = new List< object >();

			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				foreach ( Group iteeGroup in context.Groups.Include( group => group.Items ) )
				{
					returnedList.Add( iteeGroup );

					foreach ( Item iterItem in iteeGroup.Items )
					{
						returnedList.Add( iterItem );
					}
				}
			}

			return returnedList;
		}
	}
}
