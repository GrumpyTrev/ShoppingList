using Android.App;
using Android.Widget;
using ShoppingList.Core;
using ShoppingList.Core.Controllers;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	class AvailableListWrapper : ListViewWrapper <object>
	{
		public AvailableListWrapper( Activity context, ListView wrappedView, ListView revealView, string title ) : 
			base ( context, wrappedView, revealView, title )
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
				if ( PersistentStorage.IsSortedByGroup == true )
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
			return ( PersistentStorage.IsSortedByGroup == true ) ? ( ( GroupItemAdapter )WrappedView.Adapter )[ position ] :
				( ( ItemAdapter )WrappedView.Adapter )[ position ];
		}

		private void GetDataAccordingToCurrentSortState()
		{
			// Are the items shown grouped or just in item order
			if ( PersistentStorage.IsSortedByGroup == true )
			{
				WrappedView.Adapter = new GroupItemAdapter( Context, ListingController.GetGroupsAndItems() );
			}
			else
			{
				WrappedView.Adapter = new ItemAdapter( Context, ListingController.GetItems() );
			}
		}
	}
}