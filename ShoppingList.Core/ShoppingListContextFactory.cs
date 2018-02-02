using System;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Core.Model;
using ShoppingList.Core.Services;

namespace ShoppingList.Core
{
	/// <summary>
	/// A factory to create an instance of the ShoppingListContext 
	/// </summary>
	public static class ShoppingListContextFactory
	{
		public static ShoppingListContext Create()
		{
			// Pass the SQLite connection options to the context
			ShoppingListContext context = new ShoppingListContext(
				new DbContextOptionsBuilder<ShoppingListContext>().
				UseSqlite( $"Filename={ConfigurationService.FullDatabasePath}" ).Options );

			// Ensure that the SQLite database and sechema is created!
			context.Database.EnsureCreated();

			return context;
		}
	}
}
