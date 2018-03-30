using Android.App;
using Android.Widget;
using ShoppingList.Core.Controllers;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class CurrentListWrapper : ListViewWrapper< ListItem >
	{
		public CurrentListWrapper( Activity context, ListView wrappedView, ListView revealView, string title ) : 
			base ( context, wrappedView, revealView, title )
		{
			WrappedView.Adapter = new ListItemAdapter( Context, ShoppingController.GetCurrentListItems() );
		}

		public override void DataSetChanged()
		{
			( ( ListItemAdapter )WrappedView.Adapter ).Items = ShoppingController.GetCurrentListItems();
			( ( ListItemAdapter )WrappedView.Adapter ).NotifyDataSetChanged();
		}

		protected override void InitialiseTouchHandler()
		{
			ListenForRightItemSwipe();
			ListenForRightReveal();
		}

		protected override ListItem GetDataItem( int position )
		{
			return ( ( ListItemAdapter )WrappedView.Adapter )[ position ];
		}
	}
}