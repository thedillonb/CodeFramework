using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using CodeFramework.Cells;
using MonoTouch.Dialog.Utilities;

namespace CodeFramework.Elements
{
	
	public class RepositoryElement : Element, IElementSizing, IColorizeBackground, IImageUpdated
	{       
		private string _name;
		private int _followers;
		private int _forks;
		private string _description;
		private string _owner;
        private UIImage _image;
        private Uri _logo;
		
		public UITableViewCellStyle Style { get; set;}
		public UIColor BackgroundColor { get; set; }
		public bool ShowOwner { get; set; }
		

		public RepositoryElement(string name, int followers, int forks, string description, string owner, Uri logo)
			: base(null)
		{
			_name = name;
			_followers = followers;
			_forks = forks;
			_description = description;
			_owner = owner;
            _logo = logo;
			this.Style = UITableViewCellStyle.Default;
			ShowOwner = true;

		}
		
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
            var descriptionHeight = 0f;
            if (!string.IsNullOrEmpty(_description))
                descriptionHeight = _description.MonoStringHeight(UIFont.SystemFontOfSize(13f), tableView.Bounds.Width - 56f - 28f) + 9f;
            return 52f + descriptionHeight;
		}
		
		protected override NSString CellKey {
			get {
				return new NSString("RepositoryCellView");
			}
		}
		
		
		public event NSAction Tapped;
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell(CellKey) as RepositoryCellView;
			if (cell == null)
				cell = RepositoryCellView.Create();
			return cell;
		}
		
		public override bool Matches(string text)
		{
			return _name.ToLower().Contains(text.ToLower());
		}
		
		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			base.Selected(dvc, tableView, path);
			if (Tapped != null)
				Tapped();
			tableView.DeselectRow (path, true);
		}
		
		void IColorizeBackground.WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			var c = cell as RepositoryCellView;
			if (c != null)
            {
                _image = _logo == null ? null : ImageLoader.DefaultRequestImage(_logo, this);
				c.Bind(_name, _followers.ToString(), _forks.ToString(), _description, ShowOwner ? _owner : null, _image);
            }
		}

        #region IImageUpdated implementation

        public void UpdatedImage(Uri uri)
        {
            var img = ImageLoader.DefaultRequestImage(uri, this);
            if (img == null)
            {
                Console.WriteLine("DefaultRequestImage returned a null image");
                return;
            }
            _image = img;

            if (uri == null)
                return;
            var root = GetImmediateRootElement ();
            if (root == null || root.TableView == null)
                return;
            root.TableView.ReloadRows (new NSIndexPath [] { IndexPath }, UITableViewRowAnimation.None);
        }

        #endregion
	}
}

