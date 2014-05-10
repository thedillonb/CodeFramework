﻿using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeFramework.iOS.TableViewCells;

namespace CodeFramework.iOS.Elements
{
    public class MilestoneElement : Element
    {
        private readonly string _title;
        private readonly int _openIssues;
        private readonly int _closedIssues;
        private readonly DateTimeOffset? _dueDate;

        public event Action Tapped;

        public UITableViewCellAccessory Accessory = UITableViewCellAccessory.None;

        public int Number { get; private set; }

        public MilestoneElement(int number, string title, int openIssues, int closedIssues, DateTimeOffset? dueDate)
            : base(title)
        {
            Number = number;
            _title = title;
            _openIssues = openIssues;
            _closedIssues = closedIssues;
            _dueDate = dueDate;
        }

        protected override NSString CellKey
        {
            get { return new NSString("milestone"); }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CellKey) as MilestoneTableViewCell ?? new MilestoneTableViewCell
            {
                SelectionStyle = UITableViewCellSelectionStyle.Blue
            };

            cell.Accessory = Accessory;
            cell.Init(_title, _openIssues, _closedIssues, _dueDate);
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            var handle = Tapped;
            if (handle != null)
                handle();
            tableView.DeselectRow(path, true);
        }
    }
}

