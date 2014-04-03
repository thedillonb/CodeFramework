using System;
using System.Drawing;
using CodeFramework.iOS.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace CodeFramework.iOS.ViewControllers
{
	public class Composer : UIViewController
	{
        protected UIBarButtonItem SendItem;
		UIViewController _previousController;
        public Action<string> ReturnAction;
        protected readonly UITextView TextView;
        protected UIView ScrollingToolbarView;

        public bool EnableSendButton
        {
            get { return SendItem.Enabled; }
            set { SendItem.Enabled = value; }
        }

		public Composer () : base (null, null)
		{
            Title = "New Comment".t();
			EdgesForExtendedLayout = UIRectEdge.None;

			var close = new UIBarButtonItem (Theme.CurrentTheme.CancelButton, UIBarButtonItemStyle.Plain, (s, e) => CloseComposer());
            NavigationItem.LeftBarButtonItem = close;
			SendItem = new UIBarButtonItem (Theme.CurrentTheme.SaveButton, UIBarButtonItemStyle.Plain, (s, e) => PostCallback());
            NavigationItem.RightBarButtonItem = SendItem;

            TextView = new UITextView(ComputeComposerSize(RectangleF.Empty));
            TextView.Font = UIFont.SystemFontOfSize(18);
            TextView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            // Work around an Apple bug in the UITextView that crashes
            if (MonoTouch.ObjCRuntime.Runtime.Arch == MonoTouch.ObjCRuntime.Arch.SIMULATOR)
                TextView.AutocorrectionType = UITextAutocorrectionType.No;

            View.AddSubview (TextView);
		}

        public static UIButton CreateAccessoryButton(string title, Action action)
        {
            var btn = new UIButton(UIButtonType.System);
            btn.SetTitle(title, UIControlState.Normal);
            btn.BackgroundColor = UIColor.White;
            btn.Layer.CornerRadius = 5f;
            btn.Layer.ShadowOffset = new SizeF(0, 0);
            btn.Layer.ShadowOpacity = 0.4f;
            btn.Layer.ShadowColor = UIColor.Black.CGColor;
            btn.Layer.ShadowRadius = 1f;
            btn.TouchUpInside += (object sender, System.EventArgs e) => action();
            return btn;
        }

        public void SetAccesoryButtons(IEnumerable<UIButton> buttons)
        {
            ScrollingToolbarView = new ScrollingToolbarView(new RectangleF(0, 0, View.Bounds.Width, 40f), buttons);
            ScrollingToolbarView.BackgroundColor = UIColor.White;
            TextView.InputAccessoryView = ScrollingToolbarView;
        }

        public string Text
        {
            get { return TextView.Text; }
            set { TextView.Text = value; }
        }

		public void CloseComposer ()
		{
			SendItem.Enabled = true;
			_previousController.DismissViewController(true, null);
        }

		void PostCallback ()
		{
			SendItem.Enabled = false;
            if (ReturnAction != null)
                ReturnAction(Text);
		}
		
		void KeyboardWillShow (NSNotification notification)
		{
		    var nsValue = notification.UserInfo.ObjectForKey (UIKeyboard.BoundsUserInfoKey) as NSValue;
		    if (nsValue == null) return;
		    var kbdBounds = nsValue.RectangleFValue;
            UIView.Animate(1.0f, 0, UIViewAnimationOptions.CurveEaseIn, () => TextView.Frame = ComputeComposerSize (kbdBounds), null);
		}

        void KeyboardWillHide (NSNotification notification)
        {
            TextView.Frame = ComputeComposerSize(new RectangleF(0, 0, 0, 0));
        }

	    RectangleF ComputeComposerSize (RectangleF kbdBounds)
		{
			var view = View.Bounds;
            return new RectangleF (0, 0, view.Width, view.Height-kbdBounds.Height);
		}

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
		
        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UIKeyboardWillShowNotification"), KeyboardWillShow);
            NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UIKeyboardWillHideNotification"), KeyboardWillHide);
            TextView.BecomeFirstResponder ();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }
		
		public void NewComment (UIViewController parent, Action<string> action)
		{
            Title = Title;
            ReturnAction = action;
            _previousController = parent;
            TextView.BecomeFirstResponder ();
            var nav = new UINavigationController(this);
            parent.PresentViewController(nav, true, null);
		}
	}
}
