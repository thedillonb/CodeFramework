using MonoTouch.UIKit;

// ReSharper disable once CheckNamespace
namespace CodeFramework.iOS
{
    public static class Images
    {
        public static UIImage LoginUserUnknown { get { return UIImageHelper.FromFileAuto("Images/login_user_unknown"); } }
        public static UIImage Cancel { get { return UIImage.FromBundle("Images/cancel"); } }
        public static UIImage BackButton { get { return UIImage.FromBundle("Images/back"); } }
        public static UIImage MenuButton { get { return UIImage.FromBundle("Images/three_lines"); } }
    }
}