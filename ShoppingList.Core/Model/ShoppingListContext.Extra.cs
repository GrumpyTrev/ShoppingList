using Microsoft.EntityFrameworkCore;

namespace ShoppingList.Core.Model
{
    public partial class ShoppingListContext : DbContext
    {
		public ShoppingListContext( DbContextOptions<ShoppingListContext> options ) : base( options )
		{
		}
    }
}
