using CodeFramework.Views;
using MonoTouch;
using System.Threading.Tasks;
using System;

namespace CodeFramework.Controllers
{
    public abstract class BaseControllerDrivenViewController : BaseDialogViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;

        public IController Controller { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        protected BaseControllerDrivenViewController(bool push = true, bool refresh = true)
            : base(push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => Controller.Update(true);
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
                    Controller.Refresh();
                    ReloadComplete(); 
                }
                else
                {
                    Controller.Update(false);
                }

                _firstSeen = true;
            }
        }

        public void ShowLoading(bool background, Action loadAction)
        {
            if (background)
            {
                this.DoWorkNoHud(() => {
                    loadAction();
                }, (e) => { }, ReloadComplete);
            }
            else
            {
                this.DoWork("Loading...", () => {
                    loadAction();
                }, (e) => { }, ReloadComplete);

            }
        }
    }
}

