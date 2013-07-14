using MonoTouch.Dialog;
using System;

namespace CodeFramework.Elements
{
    public class TrueFalseElement : BooleanElement
    {
        public TrueFalseElement(string caption, bool value, Action<BooleanElement> changeAction = null)
            : base(caption, value)
        {
            if (changeAction != null)
                this.ValueChanged += (object sender, EventArgs e) => changeAction(this);
        }

        public override MonoTouch.UIKit.UITableViewCell GetCell(MonoTouch.UIKit.UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.BackgroundColor = StyledElement.BgColor;
            cell.TextLabel.Font = StyledElement.DefaultTitleFont;
            cell.TextLabel.TextColor = StyledElement.DefaultTitleColor;
            return cell;
        }
    }
}

