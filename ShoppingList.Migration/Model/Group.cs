using System;
using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
    public partial class Group
    {
        public Group()
        {
            Item = new HashSet<Item>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? Colour { get; set; }

        public ICollection<Item> Item { get; set; }
    }
}
