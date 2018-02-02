using System;
using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
    public partial class List
    {
        public List()
        {
            ItemInList = new HashSet<ItemInList>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<ItemInList> ItemInList { get; set; }
    }
}
