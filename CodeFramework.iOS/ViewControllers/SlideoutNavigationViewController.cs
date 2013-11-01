using MonoTouch.UIKit;
using CodeFramework.Views;

namespace CodeFramework.ViewControllers
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

            //Set the menu navigation background
            SetMenuNavigationBackgroundImage(Theme.CurrentTheme.MenuNavbarBackground, UIBarMetrics.Default);
        }

        /// <summary>
        /// Creates the menu button.
        /// </summary>
        /// <returns>The menu button.</returns>
        protected override UIBarButtonItem CreateMenuButton()
        {
            return new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.ThreeLinesButton, Show));
        }
    }
}

