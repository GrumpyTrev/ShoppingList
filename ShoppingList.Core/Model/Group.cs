using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
	public partial class Group
    {
        public Group()
        {
            Items = new HashSet<Item>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? Colour { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
