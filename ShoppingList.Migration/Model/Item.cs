using System;
using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
    public partial class Item
    {
        public Item()
        {
            ItemInList = new HashSet<ItemInList>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long GroupId { get; set; }

        public Group Group { get; set; }
        public ICollection<ItemInList> ItemInList { get; set; }
    }
}
