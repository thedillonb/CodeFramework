using System;
using CodeFramework.Elements;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;

namespace CodeFramework.Controllers
{
    /// <summary>
    /// I screwed up big time. The Controller class used generics to store the Model. However, since the last MonoTouch
    /// update, generic classes that inheirt from NSObject caused the app to crash so I needed to remake it. Unfortunately,
    /// this was at a time where the app I had submitted to the app store got rejected so I needed to whip up a temp class
    /// to make everything work reasonably well so this is basically a dupe of Controller.cs
    /// </summary>
    public abstract class BaseModelDrivenController : BaseDialogViewController
    {
        protected ErrorView CurrentError;
        private object _model;
        private Type _modelType;
        private bool _firstSeen;

        // This must be a generic type because I can't template this controller
        protected object Model
        {
            get 
            { 
                return _model; 
            }
            set 
            {
                if (value == null || value.GetType() == _modelType)
                {
                    _model = value;
                }
                else
                {
                    throw new InvalidOperationException("Can only assign model if model type is equal to the assigned type.");
                }
            }
        }

        public bool Loaded { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        protected BaseModelDrivenController(Type modelType, bool push = true, bool refresh = true)
            : base(push)
        {
            _modelType = modelType;
            if (refresh)
                RefreshRequested += (sender, e) => UpdateAndRender(false);
        }

        //Called when the UI needs to be updated with the model data            
        protected abstract void OnRender();

        //Called when the controller needs to request the model from the server
        protected abstract object OnUpdateModel(bool forced);

        /// <summary>
        /// Code called to render the model
        /// </summary>
        public void Render()
        {
            try
            {
                OnRender();
            }
            catch (Exception ex)
            {
                MonoTouch.Utilities.LogException("Error when refreshing view", ex);
            }

            if (TableView.TableFooterView != null)
                TableView.TableFooterView.Hidden = Root.Count == 0;
        }

        public void UpdateAndRender(bool showLoading = true)
        {
            if (CurrentError != null)
                CurrentError.RemoveFromSuperview();
            CurrentError = null;

            if (!showLoading)
            {
                this.DoWorkNoHud(() => {
                    Model = OnUpdateModel(true);
                    InvokeOnMainThread(Render);
                }, ex => Utilities.ShowAlert("Unable to refresh!", "There was an issue while attempting to refresh. " + ex.Message), ReloadComplete);
            }
            else
            {
                this.DoWork(() => {
                    Model = OnUpdateModel(false);
                    InvokeOnMainThread(Render);
                }, ex => {
                    CurrentError = ErrorView.Show(View.Superview, ex.Message);
                }, ReloadComplete);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //We only want to run this code once, when teh view is first seen...
            if (!_firstSeen)
            {
                //Check if the model is pre-loaded
                if (Model != null)
                {
                    Render();
                    ReloadComplete(); 
                }
                else
                {
                    UpdateAndRender();
                }

                _firstSeen = true;
            }
        }
    }
}

