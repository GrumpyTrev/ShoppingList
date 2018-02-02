using System;
using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
    public partial class List
    {
        public List()
        {
            ListItems = new HashSet<ListItem>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<ListItem> ListItems { get; set; }
    }
}
