using Android.App;
using Android.Widget;
using ShoppingList.Core.Controllers;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class ShoppingBasketListWrapper : ListViewWrapper< ListItem >
	{
		public ShoppingBasketListWrapper( Activity context, ListView wrappedView, ListView revealView, string title ) : 
			base ( context, wrappedView, revealView, title )
		{
			WrappedView.Adapter = new ListItemAdapter( Context, ShoppingController.GetBasketListItems() );
		}

		public override void DataSetChanged()
		{
			( ( ListItemAdapter )WrappedView.Adapter ).Items = ShoppingController.GetBasketListItems();
			( ( BaseAdapter )WrappedView.Adapter ).NotifyDataSetChanged();
		}

		protected override void InitialiseTouchHandler()
		{
			ListenForLeftReveal();
			ListenForLeftItemSwipe();
		}

		protected override ListItem GetDataItem( int position )
		{
			return ( ( ListItemAdapter )WrappedView.Adapter )[ position ];
		}
	}
}