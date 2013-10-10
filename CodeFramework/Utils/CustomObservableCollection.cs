using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;

namespace CodeFramework.Utils
{
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        public CustomObservableCollection()
            : base()
        {

        }

        public CustomObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }

        public CustomObservableCollection(List<T> list)
            : base(list)
        {

        }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) 
                throw new ArgumentNullException("collection");

            bool added = false;
            foreach (T item in collection)
            {
                this.Items.Add(item);
                added = true;
            }

            if (added)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
                // Cannot use NotifyCollectionChangedAction.Add, because Constructor supports only the 'Reset' action.
            }
        }

        public virtual void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null) 
                throw new ArgumentNullException("collection");

            bool removed = false;
            foreach (T item in collection)
                if (this.Items.Remove(item))
                    removed = true;

            if (removed)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection.ToList()));
                // Cannot use NotifyCollectionChangedAction.Remove, because Constructor supports only the 'Reset' action.
            }
        }

        public virtual void Reset(T item)
        {
            this.Reset(new List<T>() { item });
        }

        public virtual void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
                return;

            int count = this.Count;

            // Step 1: Clear the old items
            this.Items.Clear();

            // Step 2: Add new items
            foreach (T item in collection)
                this.Items.Add(item);

            // Step 3: Don't forget the event
            if (this.Count != count)
                this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

