#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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
            property = property;
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
            NotifyCollectionChangedEventArgs e = args;
            var collection = target as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        collection.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        collection.Insert(e.OldStartingIndex, item);
                    }

                    break;
            }
        }

        private void CollectionRedo()
        {
            NotifyCollectionChangedEventArgs e = args;
            var collection = target as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        collection.Insert(e.NewStartingIndex, item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        collection.Remove(item);
                    }

                    break;
            }
        }
    }
}