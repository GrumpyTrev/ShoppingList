using System;
using System.Collections.Generic;

namespace ShoppingList.Core.Model
{
    public partial class ListItem
    {
        public long Id { get; set; }
        public long ListId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }

        public Item Item { get; set; }
        public List List { get; set; }
    }
}
