using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class GroupItemAdapter: BaseAdapter< object >
	{
		Activity context = null;
		public object[] Items { get; set; }

		public GroupItemAdapter( Activity context, object[] items )
		{
			this.context = context;
			Items = items;
		}

		public override long GetItemId( int position )
		{
			return position;
		}

		public override object this[ int position ]
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
			object objectDisplay = Items[ position ];

			if ( ( objectDisplay is Group ) == true )
			{
				Group groupToDisplay = objectDisplay as Group;

				// Check if this is a group view
				if ( ( view == null ) || ( view.FindViewById< TextView>( Resource.Id.GroupName ) == null ) )
				{
					view = context.LayoutInflater.Inflate( Resource.Layout.GroupHeader, null );
				}
				view.FindViewById<TextView>( Resource.Id.GroupName ).Text = groupToDisplay.Name;
				view.FindViewById<TextView>( Resource.Id.GroupColour ).SetBackgroundColor( new Color( ( int )groupToDisplay.Colour ) );
				view.FindViewById<TextView>( Resource.Id.GroupColourFiller ).SetBackgroundColor( new Color( ( int )groupToDisplay.Colour ) );
			}
			else
			{
				Item itemToDisplay = objectDisplay as Item;

				// Check if this is an item view
				if ( ( view == null ) || ( view.FindViewById<TextView>( Resource.Id.ItemName ) == null ) )
				{
					view = context.LayoutInflater.Inflate( Resource.Layout.GroupItem, null );
				}

				view.FindViewById<TextView>( Resource.Id.ItemName ).Text = itemToDisplay.Name;
			}

			return view;
		}
	}
}