using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Cells
{
    public partial class IssueCellView : UITableViewCell
    {
        public static IssueCellView Create()
        {
            var cell = new IssueCellView();
            var views = NSBundle.MainBundle.LoadNib("IssueCellView", cell, null);
            cell = Runtime.GetNSObject( views.ValueAt(0) ) as IssueCellView;

            if (cell == null)
            {
                Console.WriteLine("Null cell!");
            }
            else
            {
                cell.Caption.TextColor = Theme.CurrentTheme.MainTitleColor;
                cell.Number.TextColor = Theme.CurrentTheme.MainTitleColor;
                cell.AddSubview(new SeperatorIssues {Frame = new RectangleF(65f, 5f, 1f, cell.Frame.Height - 10f)});
                cell.Image1.Image = Theme.CurrentTheme.IssueCellImage1;
                cell.Image2.Image = Theme.CurrentTheme.IssueCellImage2;
                cell.Image3.Image = Theme.CurrentTheme.IssueCellImage3;
                cell.Image4.Image = Theme.CurrentTheme.IssueCellImage4;
                cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);

                cell.Caption.Font = cell.Caption.Font.WithSize(cell.Caption.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.Number.Font = cell.Number.Font.WithSize(cell.Number.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.Label1.Font = cell.Label1.Font.WithSize(cell.Label1.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.Label2.Font = cell.Label2.Font.WithSize(cell.Label2.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.Label3.Font = cell.Label3.Font.WithSize(cell.Label3.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.Label4.Font = cell.Label4.Font.WithSize(cell.Label4.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                cell.IssueType.Font = cell.IssueType.Font.WithSize(cell.IssueType.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
            }

            //Create the icons
            return cell;
        }

        public IssueCellView()
        {
        }

        public IssueCellView(IntPtr handle)
            : base(handle)
        {
        }

        public void Bind(string title, string status, string priority, string assigned, string lastUpdated, string id, string kind)
        {
            Caption.Text = title;
            Label1.Text = status;
            Label2.Text = priority;
            Label3.Text = assigned;
            Label4.Text = lastUpdated;
            Number.Text = "#" + id;
            IssueType.Text = kind;

            /*
            if (model.CommentCount > 0)
            {
                var ms = model.CommentCount.ToString ();
                var ssize = ms.MonoStringLength(CountFont);
                var boxWidth = Math.Min (22 + ssize, 18);
                AddSubview(new CounterView(model.CommentCount) { Frame = new RectangleF(Bounds.Width-30-boxWidth, Bounds.Height / 2 - 8, boxWidth, 16) });
            }
            */
        }
//
//        static readonly UIFont CountFont = UIFont.BoldSystemFontOfSize (13);
//
//        private class CounterView : UIView
//        {
//            private readonly int _counter;
//            public CounterView(int counter) 
//                : base ()
//            {
//                _counter = counter;
//                BackgroundColor = UIColor.Clear;
//            }
//
//            public override void Draw(RectangleF rect)
//            {
//                if (_counter > 0){
//                    var ctx = UIGraphics.GetCurrentContext ();
//                    var ms = _counter.ToString ();
//
//                    var crect = Bounds;
//                    
//                    UIColor.Gray.SetFill ();
//                    GraphicsUtil.FillRoundedRect (ctx, crect, 3);
//                    UIColor.White.SetColor ();
//                    crect.X += 5;
//                    DrawString (ms, crect, CountFont);
//                }
//                base.Draw(rect);
//            }
//        }

        private class SeperatorIssues : UIView
        {
            public SeperatorIssues()
            {
            }

            public SeperatorIssues(IntPtr handle)
                : base(handle)
            {
            }

            public override void Draw(RectangleF rect)
            {
                base.Draw(rect);

                var context = UIGraphics.GetCurrentContext();
                //context.BeginPath();
                //context.ClipToRect(new RectangleF(63f, 0f, 3f, rect.Height));
                using (var cs = CGColorSpace.CreateDeviceRGB ())
                {
                    using (var gradient = new CGGradient (cs, new [] { 1f, 1f, 1f, 1.0f, 
                        0.7f, 0.7f, 0.7f, 1f, 
                        1f, 1f, 1.0f, 1.0f }, new [] {0, 0.5f, 1f}))
                    {
                        context.DrawLinearGradient(gradient, new PointF(0, 0), new PointF(0, rect.Height), 0);
                    }
                }
            }
        }
    }
}

