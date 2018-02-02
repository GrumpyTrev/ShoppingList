using ShoppingList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingList.Migration
{
	class Program
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "Hello World!" );
			var dbFullPath = @"C:\Android\ShoppingList\ShoppingList.db3";

			try
			{
				using ( var db = new ShoppingListContext( dbFullPath ) )
				{
					if ( db.Database.EnsureCreated() == true )
					{
						System.Diagnostics.Debug.WriteLine( "Created" );
					}

					var catsInTheBag = db.Group.ToList<Group>();
					foreach ( Group g in catsInTheBag )
					{
						System.Diagnostics.Debug.WriteLine( g.Name );
					}
				}

			}
			catch ( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString() );
			}


		}
	}
}

