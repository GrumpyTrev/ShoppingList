using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class ItemAdapter: BaseAdapter< Item >
	{
		Activity context = null;
		public Item[] Items { get; set; }

		public ItemAdapter( Activity context, Item[] items )
		{
			this.context = context;
			Items = items;
		}

		public override long GetItemId( int position )
		{
			return position;
		}

		public override Item this[ int position ]
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

			Item itemToDisplay = Items[ position ];

			view.FindViewById<TextView>( Resource.Id.ItemName ).Text = itemToDisplay.Name;
			view.FindViewById<TextView>( Resource.Id.Quantity ).Text = "";
			view.FindViewById<TextView>( Resource.Id.GroupColour ).SetBackgroundColor( new Color( ( int )itemToDisplay.Group.Colour ) );

			return view;
		}
	}
}