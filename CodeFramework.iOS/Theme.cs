using MonoTouch.UIKit;

namespace CodeFramework.iOS
{
    public static class Theme
    {
        public static ICodeFrameworkTheme CurrentTheme;
    }

    /// <summary>
    /// I'd really rather define these at the top most level available to the app. A.k.a the actually application
    /// code itself instead of the library. All theme tuning will be done on that level.
    /// </summary>
    public interface ICodeFrameworkTheme
    {
        UIImage BackButton { get; }
        UIImage ThreeLinesButton { get; }
        UIImage CancelButton { get; }
        UIImage SaveButton { get; }
        UIImage ViewButton { get; }
        UIImage WebBackButton { get; }
        UIImage WebFowardButton { get; }
        UIImage ForkButton { get; }

        UIImage AnonymousUserImage { get; }

        UIImage IssueCellImage1 { get; }
        UIImage IssueCellImage2 { get; }
        UIImage IssueCellImage3 { get; }
        UIImage IssueCellImage4 { get; }

        UIImage RepositoryCellFollowers { get; }
        UIImage RepositoryCellForks { get; }
        UIImage RepositoryCellUser { get; }

        UIColor MainTitleColor { get; }
        UIColor MainSubtitleColor { get; }
        UIColor MainTextColor { get; }
        UIColor ViewBackgroundColor { get; }
        UIImage MenuBackground { get; }

        UIColor WebButtonTint { get; }

		UIColor AccountsNavigationBarTint { get; }
		UIColor SlideoutNavigationBarTint { get; }
		UIColor ApplicationNavigationBarTint { get; }

    }
}
