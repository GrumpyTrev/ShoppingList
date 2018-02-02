using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class ListItemAdapter: BaseAdapter< ListItem >
	{
		Activity context = null;
		public ListItem[] Items { get; set; }

		public ListItemAdapter( Activity context, ListItem[] items )
		{
			this.context = context;
			Items = items;
		}

		public override long GetItemId( int position )
		{
			return position;
		}

		public override ListItem this[ int position ]
		{
			get
			{
				return Items[ position ];
			}
		}

		public override int Count
		{
			get
			{
				return Items.Length;
			}
		}

		public override View GetView( int position, View convertView, ViewGroup parent )
		{
			View view = convertView;

			if ( view == null )
			{
				view = context.LayoutInflater.Inflate( Resource.Layout.ShoppingListItem, null );
			}

			ListItem itemToDisplay = Items[ position ];

			view.FindViewById<TextView>( Resource.Id.ItemName ).Text = itemToDisplay.Item.Name;
			view.FindViewById<TextView>( Resource.Id.Quantity ).Text = ( itemToDisplay.Quantity == 1 ) ? "" : itemToDisplay.Quantity.ToString();
			view.FindViewById<TextView>( Resource.Id.GroupColour ).SetBackgroundColor( new Color( ( int )itemToDisplay.Item.Group.Colour ) );

			return view;
		}
	}
}