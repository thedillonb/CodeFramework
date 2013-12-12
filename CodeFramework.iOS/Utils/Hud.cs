using BigTed;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Utils
{
	public class Hud : IHud
    {
		public static UIColor BackgroundTint;
		private readonly UIView _window;
		public Hud(UIView window)
        {
			_window = window;

        }

		public void Show(string text)
		{
			ProgressHUD.Shared.HudBackgroundColour = BackgroundTint;
			BTProgressHUD.Show(text, maskType: ProgressHUD.MaskType.Gradient);
		}

		public static void ShowSuccess(string text)
		{
			BTProgressHUD.ShowSuccessWithStatus(text);
		}

		public static void ShowFailure(string text)
		{
			BTProgressHUD.ShowErrorWithStatus(text);
		}

		public void Hide()
		{
			BTProgressHUD.Dismiss();
		}
    }

	public interface IHud
	{
		void Show(string text);

		void Hide();
	}
}

