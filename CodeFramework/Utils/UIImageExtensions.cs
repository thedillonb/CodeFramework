using System;
using MonoTouch.UIKit;

namespace MonoTouch.UIKit
{
    public static class UIImageHelper
    {
        public static UIImage FromFileAuto(string filename, string extension = "png")
        {
            if (UIScreen.MainScreen.Scale > 1.0)
            {
                var file = filename + "@2x." + extension;
                if (System.IO.File.Exists(file))
                    return UIImage.FromFile(file);
                else
                    return UIImage.FromFile(filename + "." + extension);
            }
            else
            {
                var file = filename + "." + extension;
                if (System.IO.File.Exists(file))
                    return UIImage.FromFile(file);
                else
                    return UIImage.FromFile(filename + "@2x." + extension);
            }
        }
    }
}

