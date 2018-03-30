using System;

using Android.App;
using Android.Widget;
using ShoppingList.Core.Model;

namespace ShoppingList.Droid
{
	abstract class ListViewWrapper< T >
	{
		/// <summary>
		/// Create a ListViewWrapper arround the specified views
		/// </summary>
		/// <param name="context"></param>
		/// <param name="wrappedView"></param>
		/// <param name="revealView"></param>
		/// <param name="title"></param>
		public ListViewWrapper( Activity context, ListView wrappedView, ListView revealView, string title )
		{
			// Save the passed in parameters
			WrappedView = wrappedView;
			RevealView = revealView;
			Context = context;
			ToolbarTitle = title;

			// Create a touch listener and attach it to the view
			Listener = new ListViewTouchListener( WrappedView );
			WrappedView.SetOnTouchListener( Listener );

			// Let derived classes initialise the listener
			InitialiseTouchHandler();
		}

		/// <summary>
		/// Derived classes must implement data change handling
		/// </summary>
		public abstract void DataSetChanged();

		/// <summary>
		/// The touch handler initialisation that all derived classes must implement
		/// </summary>
		protected abstract void InitialiseTouchHandler();

		/// <summary>
		/// Derived classes must provide a standard way of accessing data items
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		protected abstract T GetDataItem( int position );

		/// <summary>
		/// Pass on a view change via the RevealActive event
		/// </summary>
		protected void OnViewRevealed()
		{
			RevealActive?.Invoke( this, new EventArgs() );
		}

		/// <summary>
		/// Configure the listener to allow a right reveal as part of a left group swipe
		/// </summary>
		protected void ListenForRightReveal()
		{
			Listener.LeftSwipeAllowed = true;
			Listener.LeftGroupSwipe = true;
			Listener.RightRevealView = RevealView;

			// Hook into the FlingLeft event
			Listener.FlingLeftHandler += ( object sender, ListViewTouchListener.SwipeEventArgs args ) => 
			{
				RevealActive?.Invoke( WrappedView, new EventArgs() );
			};
		}

		/// <summary>
		/// Configure the listener to allow a left reveal as part of a right group swipe
		/// </summary>
		protected void ListenForLeftReveal()
		{
			Listener.RightSwipeAllowed = true;
			Listener.RightGroupSwipe = true;
			Listener.LeftRevealView = RevealView;

			// Hook into the FlingRight event
			Listener.FlingRightHandler += ( object sender, ListViewTouchListener.SwipeEventArgs args ) => 
			{
				RevealActive?.Invoke( WrappedView, new EventArgs() );
			};
		}

		/// <summary>
		/// Configure the listener to allow for right single item swiping
		/// </summary>
		protected void ListenForRightItemSwipe()
		{
			Listener.RightSwipeAllowed = true;
			Listener.RightGroupSwipe = false;

			// Hook into the FlingRight event
			Listener.FlingRightHandler += ( object sender, ListViewTouchListener.SwipeEventArgs args ) => 
			{
				if ( args.Position != -1 )
				{
					// Raise the ItemSwiped event
					ItemSwiped?.Invoke( WrappedView, new SwipeItemEventArgs< T > { Item = GetDataItem( args.Position ), WasFlung = args.WasFlung } );
				}
			};
		}

		/// <summary>
		/// Configure the listener to allow for left single item swiping
		/// </summary>
		protected void ListenForLeftItemSwipe()
		{
			Listener.LeftSwipeAllowed = true;
			Listener.LeftGroupSwipe = false;
			
			// Hook into the FlingLeft event
			Listener.FlingLeftHandler += ( object sender, ListViewTouchListener.SwipeEventArgs args ) => 
			{
				if ( args.Position != -1 )
				{
					// Raise the ItemSwiped event
					ItemSwiped?.Invoke( WrappedView, new SwipeItemEventArgs<T> { Item = GetDataItem( args.Position ), WasFlung = args.WasFlung } );
				}
			};
		}

		/// <summary>
		/// Swipe event arguments
		/// </summary>
		public class SwipeItemEventArgs < U > : EventArgs
		{
			public U Item { get; set; }
			public bool WasFlung { get; set; }
		}

		//
		// Properties
		//

		/// <summary>
		/// The view wrapped up by this class
		/// </summary>
		public ListView WrappedView { get; private set; }

		/// <summary>
		/// The view revealed as part of a swipe 
		/// </summary>
		protected ListView RevealView { get; private set; }

		/// <summary>
		/// The (main) toolbar title to display when this view is being displayed
		/// </summary>
		public string ToolbarTitle{ get; private set; }

		/// <summary>
		/// The event to invoke when the reveal view is shown
		/// </summary>
		public event EventHandler< EventArgs > RevealActive;

		/// <summary>
		/// The event to invoke when an item is swiped
		/// </summary>
		public event EventHandler< SwipeItemEventArgs < T > > ItemSwiped;

		/// <summary>
		/// A contect for derived classes to use
		/// </summary>
		protected Activity Context { get; private set; }

		/// <summary>
		/// The touch listener for this class
		/// </summary>
		protected ListViewTouchListener Listener { get; private set; }
	}
}