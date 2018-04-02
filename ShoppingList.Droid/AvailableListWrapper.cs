using Android.App;
using Android.Widget;
using ShoppingList.Core;
using ShoppingList.Core.Controllers;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class AvailableListWrapper : ListViewWrapper <object>
	{
		public AvailableListWrapper( Activity context, ListView wrappedView, ListView revealView, string title, ItemSort orderHandler ) : 
			base ( context, wrappedView, revealView, title, orderHandler )
		{
			GetDataAccordingToCurrentSortState();
		}

		public override void DataSetChanged()
		{
			GetDataAccordingToCurrentSortState();
			( ( BaseAdapter ) WrappedView.Adapter ).NotifyDataSetChanged();
		}

		protected override void InitialiseTouchHandler()
		{
			ListenForLeftItemSwipe();
			ListenForLeftReveal();

			Listener.ItemSelectionHandler += ( object sender, ListViewTouchListener.SelectionEventArgs args ) =>
			{
				if ( SortOrderHandler.CurrentOrder == ItemSort.SortState.Grouped )
				{
					if ( args.Position != -1 )
					{
						// Don't allow the selection of group headers
						args.Cancel = ( ( ( GroupItemAdapter )WrappedView.Adapter )[ args.Position ] is Group );
					}
				}
			};
		}

		protected override object GetDataItem( int position )
		{
			return ( SortOrderHandler.CurrentOrder == ItemSort.SortState.Grouped ) ? ( ( GroupItemAdapter )WrappedView.Adapter )[ position ] :
				( ( ItemAdapter )WrappedView.Adapter )[ position ];
		}

		private void GetDataAccordingToCurrentSortState()
		{
			// Are the items shown grouped or just in item order
			if ( SortOrderHandler.CurrentOrder == ItemSort.SortState.Grouped )
			{
				WrappedView.Adapter = new GroupItemAdapter( Context, ListingController.GetGroupsAndItems() );
			}
			else
			{
				WrappedView.Adapter = new ItemAdapter( Context, ListingController.GetItems( SortOrderHandler.CurrentOrder == ItemSort.SortState.Alphabetic ) );
			}
		}
	}
}