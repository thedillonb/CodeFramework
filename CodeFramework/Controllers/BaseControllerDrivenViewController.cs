using System;
using CodeFramework.Elements;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;
using CodeFramework.Filters.Controllers;
using CodeFramework.Filters.Models;

namespace CodeFramework.Controllers
{
    public abstract class BaseControllerDrivenViewController : BaseDialogViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;

        public IController Controller { get; protected set; }

        public bool Loaded { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        protected BaseControllerDrivenViewController(bool push = true, bool refresh = true)
            : base(push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => UpdateAndRender(true);
        }

        private void UpdateAndRender(bool force)
        {
            if (CurrentError != null)
                CurrentError.RemoveFromSuperview();
            CurrentError = null;

            if (force)
            {
                this.DoWorkNoHud(() => Controller.UpdateAndRender(force), 
                                 ex => Utilities.ShowAlert("Unable to refresh!".t(), "There was an issue while attempting to refresh. ".t() + ex.Message), 
                                 ReloadComplete);
            }
            else
            {
                this.DoWork(() => Controller.UpdateAndRender(force),
                            ex => { CurrentError = ErrorView.Show(View.Superview, ex.Message); }, 
                ReloadComplete);
            }
        }

        public void UpdateAndRender()
        {
            UpdateAndRender(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //We only want to run this code once, when teh view is first seen...
            if (!_firstSeen)
            {
                //Check if the model is pre-loaded
                if (Controller.IsModelValid)
                {
                    Controller.Render();
                    ReloadComplete(); 
                }
                else
                {
                    UpdateAndRender(false);
                }

                _firstSeen = true;
            }
        }



    }
}

