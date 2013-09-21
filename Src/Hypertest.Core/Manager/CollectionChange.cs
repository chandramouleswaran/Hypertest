using System.Collections;
using System.Collections.Specialized;

namespace Hypertest.Core.Manager
{
    public class CollectionChange : Change
    {
        public CollectionChange(object target, NotifyCollectionChangedEventArgs args)
        {
            this.args = args;
            this.target = target;
            this.property = property;
        }

        public object target { get; internal set; }

        public NotifyCollectionChangedEventArgs args { get; internal set; }

        public string property { get; internal set; }

        internal override void Undo()
        {
            CollectionUndo();
        }

        internal override void Redo()
        {
            CollectionRedo();
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
