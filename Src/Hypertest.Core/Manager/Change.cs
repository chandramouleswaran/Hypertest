using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using Hypertest.Core.Tests;
using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public class Change
    {
        public Change(object target, EventArgs args, string property="")
        {
            this.args = args;
            this.target = target;
            this.property = property;
        }

        public object target { get; internal set; }

        public EventArgs args { get; internal set; }

        public string property { get; internal set; }

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
            else if (args is RoutedPropertyChangedEventArgs<object>)
            {
                RoutedPropertyChangedEventArgs<object> e = args as RoutedPropertyChangedEventArgs<object>;
                if (e == target)
                {
                    object o1 = e.OldValue;
                    object o2 = e.NewValue;
                    object backup = o1.GetType().GetProperty(this.property).GetValue(o1);
                    o1.GetType().GetProperty(this.property).SetValue(o1, o2.GetType().GetProperty(this.property).GetValue(o2), null);
                    o2.GetType().GetProperty(this.property).SetValue(o2, backup, null);
                }
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
            else if (args is RoutedPropertyChangedEventArgs<object>)
            {
                RoutedPropertyChangedEventArgs<object> e = args as RoutedPropertyChangedEventArgs<object>;
                if (e == target)
                {
                    object o1 = e.OldValue;
                    object o2 = e.NewValue;
                    object backup = o1.GetType().GetProperty(this.property).GetValue(o1);
                    o1.GetType().GetProperty(this.property).SetValue(o1, o2.GetType().GetProperty(this.property).GetValue(o2), null);
                    o2.GetType().GetProperty(this.property).SetValue(o2, backup, null);
                }
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
