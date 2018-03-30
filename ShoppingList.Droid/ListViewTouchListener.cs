using System;
using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace ShoppingList.Droid
{
	/// <summary>
	/// The ListViewTouchListener class performs list view item swipe and fling detection
	/// </summary>
	class ListViewTouchListener: Java.Lang.Object, View.IOnTouchListener
	{
		/// <summary>
		/// Create a ListViewTouchListener for the spoecified list view
		/// </summary>
		/// <param name="parentView"></param>
		public ListViewTouchListener( ListView parentView )
		{
			monitoredView = parentView;

			// Get standard fling specific UI values
			ViewConfiguration vc = ViewConfiguration.Get( monitoredView.Context );
			wander = vc.ScaledTouchSlop;
			minimumFlingVelocity = 2500;
		}

		/// <summary>
		/// Called when a touch event has been passed to the list view for processing.
		/// Process according to the event type
		/// </summary>
		/// <param name="touchedView">The view given the event - not used assumed to be owner ShoppingListView</param>
		/// <param name="touchEvent">THe touch event</param>
		/// <returns>True if the event has been consumed and should not be further processed</returns>
		public bool OnTouch( View touchedView, MotionEvent touchEvent )
		{
			// Assume the event is not consumed
			bool touchConsumed = false;

			switch ( touchEvent.Action )
			{
				case MotionEventActions.Down:
				{
					// Adjust touch position to be relative to the parent view
					int[] viewCoords = new int[ 2 ];
					monitoredView.GetLocationOnScreen( viewCoords );
					int relX = ( int )touchEvent.RawX - viewCoords[ 0 ];
					int relY = ( int )touchEvent.RawY - viewCoords[ 1 ];

					// Work out if a child of the parent view has been selected
					// This is a bit of a brute force way of doing this but can cope with different height items.
					int childIndex = 0;
					bool childFound = false;
					Rect hitRect = new Rect();

					while ( ( childIndex < monitoredView.ChildCount ) && ( childFound == false ) )
					{
						View child = monitoredView.GetChildAt( childIndex );
						child.GetHitRect( hitRect );

						// Check if this child has been selected - all coordinates relative to the parent
						if ( hitRect.Contains( relX, relY ) == true )
						{
							childFound = true;

							// Save the view
							selectedView = child;
							selectedPosition = monitoredView.GetPositionForView( child );

							// Check if this child can be selected
							if ( ItemSelectionHandler != null )
							{
								SelectionEventArgs args = new SelectionEventArgs { Position = selectedPosition, ViewSelected = child };
								ItemSelectionHandler.Invoke( this, args );
								if ( args.Cancel == true )
								{
									selectedView = null;
								}
							}
						}
						else
						{
							childIndex++;
						}
					}

					// If a child hasn't been selected check whether or not group swipes are allowed
					if ( childFound == false )
					{
						selectedPosition = -1;

						// If group swiping is allowed (in either direction) then specify the parent view as the selected view
						if ( ( RightGroupSwipe == true ) || ( LeftGroupSwipe == true ) )
						{
							selectedView = monitoredView;
						}
					}

					// If a view has been selected then set up a velocity tracker and record the initial x coordinate
					if ( selectedView != null )
					{
						velocity = VelocityTracker.Obtain();
						velocity.AddMovement( touchEvent );

						// Record the raw starting X position to determine how far to translate the view in response to user
						// moves
						initialDownX = touchEvent.RawX;

						touchConsumed = true;
					}

					break;
				}

				case MotionEventActions.Move:
				{
					// If a view has been selected then determine how far it has been moved
					if ( selectedView != null )
					{
						// Add the current movement to those held by the tracker 
						velocity.AddMovement( touchEvent );

						float deltaX = touchEvent.RawX - initialDownX;

						if ( swiping == false )
						{
							// Not already swiping. Check if the movement is deliberate and is more horizontal than vertical
							// Get the velocity in pixels / sec

							// Check if swiping is actually allowed
							if ( SwipingAllowed( deltaX ) == true )
							{
								velocity.ComputeCurrentVelocity( 1000 );

								if ( ( Math.Abs( deltaX ) > wander ) &&
									( Math.Abs( velocity.XVelocity ) > Math.Abs( velocity.YVelocity ) ) )
								{
									swiping = true;
								}
							}
						}

						if ( swiping == true )
						{
							// Move the view unless swiping is not allowed
							if ( SwipingAllowed( deltaX ) == true )
							{
								// Move the selected view
								// If this is a group swipe then move the parent view instead
								if ( ( ( deltaX > 0 ) && ( RightGroupSwipe == true ) ) ||
									( ( deltaX < 0 ) && ( LeftGroupSwipe == true ) ) )
								{
									monitoredView.TranslationX = deltaX;

									// If moving left and there is a right reveal view then move it as well
									if ( ( deltaX < 0 ) && ( RightRevealView != null ) )
									{
										RightRevealView.TranslationX = monitoredView.Width + deltaX;
										RightRevealView.Visibility = ViewStates.Visible;
									}

									// If moving right and there is a left reveal view then move it as well
									if ( ( deltaX > 0 ) && ( LeftRevealView != null ) )
									{
										LeftRevealView.TranslationX = deltaX - monitoredView.Width;
										LeftRevealView.Visibility = ViewStates.Visible;
									}

									// If we've been moving an individual child then make sure it is put back where
									// we found it. Don't so this if the selected child is the parent
									if ( ( selectedView != monitoredView ) && ( selectedView.TranslationX != 0 ) )
									{
										selectedView.TranslationX = 0;
									}
								}
								else
								{
									// Only move this view if it is not the parent view
									if ( selectedView != monitoredView )
									{
										selectedView.TranslationX = deltaX;
									}
								}
							}

							touchConsumed = true;
						}
					}

					break;
				}

				case MotionEventActions.Up:
				{
					if ( swiping == true )
					{
						// Assume no fling detected
						bool flingRightDetected = false;
						bool flingLeftDetected = false;

						// Assume a slow drag rather than a fast fling
						bool wasFlung = false;

						// Add the current movement to those held by the tracker and detemine the curent velocities
						velocity.AddMovement( touchEvent );
						velocity.ComputeCurrentVelocity( 1000 );

						float absXVelocity = Math.Abs( velocity.XVelocity );
						float absYVelocity = Math.Abs( velocity.YVelocity );

						float deltaX = touchEvent.RawX - initialDownX;

						// First of all detect a fling.
						// If the absolute velocity is greater than the fling minimum and the movement is still
						// in a mainly horizontal direction then its a fling.
						// Also ensure that the current delta is in the same direction as the fling, i.e. make sure that
						// the view has been repositioned back to where it started before processing a fling in the opposite
						// direction
						if ( ( absXVelocity > minimumFlingVelocity ) && ( absXVelocity > ( absYVelocity * 2 ) ) )
						{
							// Check if the fling is in an allowed direction - may have changed direction since last move
							if ( SwipingAllowed( velocity.XVelocity ) == true )
							{
								// Make sure that the fling direction agrees with the current position of the view
								if ( Math.Sign( velocity.XVelocity ) == Math.Sign( deltaX ) )
								{
									flingRightDetected = ( velocity.XVelocity > 0 );
									flingLeftDetected = ( velocity.XVelocity < 0 );
									wasFlung = true;
								}
							}
						}
						// More than half the width moved?
						else if ( ( SwipingAllowed( deltaX ) == true ) && ( Math.Abs( deltaX ) > ( monitoredView.Width / 2 ) ) )
						{
							flingRightDetected = ( deltaX > 0 );
							flingLeftDetected = ( deltaX < 0 );
						}

						// Animate the selected view either to the right, left or back to the start
						// Work out whether the selected child or the parent view needs to be animated
						// Assume the child
						View animatedView = selectedView;
						if ( ( ( deltaX > 0 ) && ( RightGroupSwipe == true ) ) ||
							( ( deltaX < 0 ) && ( LeftGroupSwipe == true ) ) )
						{
							animatedView = monitoredView;
						}

						// If the animated view has not changed position then skip the animation processing
						// Thius assumes that a velocity based fling will only occur after the view has
						// moved a little first TBD
						if ( animatedView.TranslationX != 0 )
						{
							if ( flingRightDetected == true ) 
							{
								// Animate the view fully off to the right and report the event back to the view at the 
								// end of the animation
								SimpleAnimate( animatedView, monitoredView.Width, 
									new ObjectAnimatorListenerAdapter( ( animation ) =>
									{
										FlingRightHandler?.Invoke( this, new SwipeEventArgs{Position = selectedPosition, WasFlung = wasFlung }  );
										if ( animatedView != monitoredView )
										{
											animatedView.TranslationX = 0;
										}
									} ) );

								// If this is a parent view swipe and there's a left reveal then animate it across as well
								if ( ( animatedView == monitoredView ) && ( LeftRevealView != null ) )
								{
									SimpleAnimate( LeftRevealView, 0, null );
								}
							}
							else if ( flingLeftDetected == true )
							{
								// Animate the view fully off to the left and report the event back to the view at the 
								// end of the animation
								SimpleAnimate( animatedView, -monitoredView.Width,
									new ObjectAnimatorListenerAdapter( ( animation ) =>
									{
										FlingLeftHandler?.Invoke( this, new SwipeEventArgs { Position = selectedPosition, WasFlung = wasFlung } );
										if ( animatedView != monitoredView )
										{
											animatedView.TranslationX = 0;
										}
									} ) );

								// If this is a parent view swipe and there's a right reveal then animate it across as well
								if ( ( animatedView == monitoredView ) && ( RightRevealView != null ) )
								{
									SimpleAnimate( RightRevealView, 0, null );
								}
							}
							else
							{
								// No fling so animate the views back to their starting positions
								SimpleAnimate( animatedView, 0, null );

								// If the parent view was being moved and there's a left or right review then animate them back
								// to their starting positions as well
								if ( animatedView == monitoredView )
								{
									if ( ( deltaX > 0 ) && ( LeftRevealView != null ) )
									{
										// Animate the view back to the left
										SimpleAnimate( LeftRevealView, -monitoredView.Width, null );
									}

									if ( ( deltaX < 0 ) && ( RightRevealView != null ) )
									{
										// Animate the view back to the right
										SimpleAnimate( RightRevealView, monitoredView.Width, null );
									}
								}
							}
						}
					}

					// Reset state variables
					selectedView = null;
					swiping = false;

					// Free up the VelocityTracker
					if ( velocity != null )
					{
						velocity.Recycle();
						velocity = null;
					}

					break;
				}
			}

			return touchConsumed;
		}

		/// <summary>
		/// Check if the left/right swiping allowed flags allows the current movement to be responded to
		/// </summary>
		/// <param name="delta">The amount the touch point has moved since the view was first touched</param>
		/// <returns></returns>
		private bool SwipingAllowed( float delta )
		{
			return ( ( ( LeftSwipeAllowed == true ) && ( delta < 0 ) ) || ( ( RightSwipeAllowed == true ) && ( delta > 0 ) ) );
		}

		private static void SimpleAnimate( View viewToAnimate, int finalTranslate, ObjectAnimatorListenerAdapter listener )
		{
			viewToAnimate.Animate()
				.TranslationX( finalTranslate )
				.Alpha( 1 )
				.SetDuration( 200 )
				.SetListener( listener );
		}

		/// <summary>
		/// An animation listener to catch the end of the animation
		/// </summary>
		private class ObjectAnimatorListenerAdapter: AnimatorListenerAdapter
		{
			/// <summary>
			/// Create the listener and set the end of animiation action
			/// </summary>
			/// <param name="animation"></param>
			public ObjectAnimatorListenerAdapter( Action<Animator> animation ) => AnimationEnd = animation;

			/// <summary>
			/// Call the AnimationEnd action at the end of the animiation
			/// </summary>
			/// <param name="animator"></param>
			public override void OnAnimationEnd( Animator animator )
			{
				AnimationEnd( animator );
			}

			/// <summary>
			/// The action to call at the end of the animation
			/// </summary>
			private Action<Animator> AnimationEnd
			{
				get; set;
			}
		}

		/// <summary>
		/// The container view being monitored
		/// </summary>
		private ListView monitoredView = null;

		/// <summary>
		/// The child view that has been selected by a down action
		/// </summary>
		private View selectedView = null;

		/// <summary>
		/// The index of the selected child in the owner view
		/// </summary>
		private int selectedPosition = -1;

		/// <summary>
		/// Is swiping of a child item in progress
		/// </summary>
		private bool swiping = false;

		/// <summary>
		/// The raw x coordinate when the child view is first selected
		/// </summary>
		private float initialDownX = 0;

		/// <summary>
		/// VelocityTracker used to detect how quickly the child view is being moved
		/// </summary>
		private VelocityTracker velocity = null;

		/// <summary>
		/// How much can a touch wander before a swipe action can be assumed
		/// </summary>
		private int wander = 0;

		/// <summary>
		/// Minimum velocity to initiate a fling, as measured in pixels per second. 
		/// </summary>
		private int minimumFlingVelocity = 0;

		/// <summary>
		/// Is left swiping allowed
		/// </summary>
		public bool LeftSwipeAllowed 
		{
			get; set;
		} = false;

		/// <summary>
		/// Is right swiping allowed
		/// </summary>
		public bool RightSwipeAllowed
		{
			get; set;
		} = false;

		/// <summary>
		/// Is the parent view swiped when going right
		/// </summary>
		public bool RightGroupSwipe
		{
			get; set;
		} = false;

		/// <summary>
		/// Is the parent view swiped when going left
		/// </summary>
		public bool LeftGroupSwipe
		{
			get; set;
		} = false;

		public View RightRevealView
		{
			get; set;
		} = null;

		public View LeftRevealView
		{
			get; set;
		} = null;

		/// <summary>
		/// The fling right event handler
		/// </summary>
		public event EventHandler< SwipeEventArgs > FlingRightHandler;

		/// <summary>
		/// The fling left event handler
		/// </summary>
		public event EventHandler< SwipeEventArgs > FlingLeftHandler;

		/// <summary>
		/// Swipe event arguments
		/// </summary>
		public class SwipeEventArgs : EventArgs
		{
			public int Position { get; set; }
			public bool WasFlung { get; set; }
		}

		public event EventHandler< SelectionEventArgs > ItemSelectionHandler;

		/// <summary>
		/// Swipe event arguments
		/// </summary>
		public class SelectionEventArgs: EventArgs
		{
			public int Position { get; set; }
			public View ViewSelected { get; set; }
			public bool Cancel{ get; set; }
		}
	}
}