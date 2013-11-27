using CodeFramework.iOS.Views;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.ViewControllers
{
    public class SlideoutNavigationViewController : MonoTouch.SlideoutNavigation.SlideoutNavigationController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFramework.Controllers.SlideoutNavigationController" />
        ///   class.
        /// </summary>
        public SlideoutNavigationViewController()
        {
            //Setting the height to a large amount means that it will activate the slide pretty much whereever your finger is on the screen.
            SlideHeight = 9999f;
			LayerShadowing = true;
			ShadowOpacity = 0.3f;
        }

		protected override UIBarButtonItem CreateLeftMenuButton()
		{
			return new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.ThreeLinesButton, ShowMenuLeft));
		}
    }
}

