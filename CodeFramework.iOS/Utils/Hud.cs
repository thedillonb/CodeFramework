using System;
using MBProgressHUD;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Utils
{
	public class Hud : IHud
    {
		private readonly MTMBProgressHUD _hud;
		private readonly UIView _window;
		public Hud(UIView window)
        {
			_window = window;
			_hud = new MTMBProgressHUD(_window) {
				Mode = MBProgressHUDMode.Indeterminate, 
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.MBProgressHUDAnimationFade
			};
        }

		public void Show(string text)
		{
			_window.AddSubview(_hud);
			_hud.LabelText = text;
			_hud.Show(true);
		}

		public void Hide()
		{
			_hud.Hide(true);
		}
    }

	public interface IHud
	{
		void Show(string text);

		void Hide();
	}
}

