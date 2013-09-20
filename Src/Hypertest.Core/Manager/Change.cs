using System;
using System.Collections;
using System.Collections.Specialized;
using Hypertest.Core.Tests;
using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public class Change
    {
        public Change(object target, EventArgs args)
        {
            this.args = args;
            this.target = target;
        }

        public object target { get; internal set; }

        public EventArgs args { get; internal set; }

        internal void Undo()
        {
            if (args is PropertyChangedExtendedEventArgs)
            {
                PropertyUndo();
            }
            else if (args is NotifyCollectionChangedEventArgs)
            {
                CollectionUndo();
            }
        }

        internal void Redo()
        {
            if (args is PropertyChangedExtendedEventArgs)
            {
                PropertyRedo();
            }
            else if (args is NotifyCollectionChangedEventArgs)
            {
                CollectionRedo();
            }
        }

        internal void PropertyUndo()
        {
            PropertyChangedExtendedEventArgs e = args as PropertyChangedExtendedEventArgs;
            target.GetType().GetProperty(e.PropertyName).SetValue(target, e.OldValue, null);
        }
        internal void PropertyRedo()
        {
            PropertyChangedExtendedEventArgs e = args as PropertyChangedExtendedEventArgs;
            target.GetType().GetProperty(e.PropertyName).SetValue(target, e.NewValue, null);
        }


        private void CollectionUndo()
        {
            NotifyCollectionChangedEventArgs e = args as NotifyCollectionChangedEventArgs;
            IList collection = target as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        collection.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        collection.Insert(e.OldStartingIndex, item);
                    }

                    break;
            }
        }

        private void CollectionRedo()
        {
            NotifyCollectionChangedEventArgs e = args as NotifyCollectionChangedEventArgs;
            IList collection = target as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        collection.Insert(e.NewStartingIndex, item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        collection.Remove(item);
                    }

                    break;
            }
        }
    }
}
