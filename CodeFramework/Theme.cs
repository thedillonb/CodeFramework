using MonoTouch.UIKit;

namespace CodeFramework
{
    public class Theme
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
        UIImage EditButton { get; }
        UIImage SaveButton { get; }
        UIImage AddButton { get; }
        UIImage FilterButton { get; }
        UIImage GearButton { get; }
        UIImage ViewButton { get; }
        UIImage WebBackButton { get; }
        UIImage WebFowardButton { get; }

        UIImage MenuSectionBackground { get; }
        UIImage MenuNavbarBackground { get; }
        UIImage WarningImage { get; }
        UIImage ViewBackground { get; }
        UIImage AnonymousUserImage { get; }
        UIImage DropbarBackground { get; }

        UIImage TableViewSectionBackground { get; }

        UIImage IssueCellImage1 { get; }
        UIImage IssueCellImage2 { get; }
        UIImage IssueCellImage3 { get; }
        UIImage IssueCellImage4 { get; }
        
        UIImage RepositoryCellFollowers { get; }
        UIImage RepositoryCellForks { get; }
        UIImage RepositoryCellUser { get; }

    }
}
