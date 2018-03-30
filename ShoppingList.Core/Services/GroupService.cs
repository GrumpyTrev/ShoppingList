using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
	internal class GroupService: IGroupService
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
				foreach ( Group iteeGroup in context.Groups.Include( group => group.Items ).OrderBy( group => group.Name ) )
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

		public List<Item> GetItems()
		{
			List<Item> returnedList = new List<Item>();

			using ( ShoppingListContext context = ShoppingListContextFactory.Create() )
			{
				foreach ( Group iteeGroup in context.Groups.Include( group => group.Items ).OrderBy( group => group.Name ) )
				{
					foreach ( Item iterItem in iteeGroup.Items )
					{
						returnedList.Add( iterItem );
					}
				}
			}

			return returnedList.OrderBy( item => item.Name ).ToList();
		}

	}
}
