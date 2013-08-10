using System;
using MonoTouch.Foundation;

public static class LocalizationExtensions
{
    /// <summary>
    /// Gets the localized text for the specified string.
    /// </summary>
    public static string t(this string text)
    {
        if (String.IsNullOrEmpty (text))
            return text;
        return NSBundle.MainBundle.LocalizedString (text, String.Empty, String.Empty);
    }
}

