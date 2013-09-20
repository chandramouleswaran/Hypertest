using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Manager
{
    public class StateManager
    {
        private Stack<ChangeSet> _undoStack;
        private Stack<ChangeSet> _redoStack;
        private bool _isWorking;
        private ObservableCollection<INotifyPropertyChanged> _targets;
        private ObservableCollection<INotifyCollectionChanged> _collections;

        public StateManager()
        {
            _undoStack = new Stack<ChangeSet>();
            _redoStack = new Stack<ChangeSet>();
            _isWorking = false;
            _targets = new ObservableCollection<INotifyPropertyChanged>();
            _collections = new ObservableCollection<INotifyCollectionChanged>();
        }

        public IEnumerable<ChangeSet> UndoStack { get { return _undoStack; } }
        public IEnumerable<ChangeSet> RedoStack { get { return _redoStack; } }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public void Dispose()
        {
            DetachCollectionChanged(_collections);
            DetachPropertyChanged(_targets);
        }

        public void MonitorCollection(INotifyCollectionChanged collection)
        {
            if (!_collections.Contains(collection))
            {
                _collections.Add(collection);
                collection.CollectionChanged += collection_CollectionChanged;
            }
        }

        public void MonitorObject(INotifyPropertyChanged target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
                target.PropertyChanged += target_PropertyChanged;
            }
        }

        public bool CanUndo()
        {
            return _undoStack.Count > 0;
        }

        public void Undo()
        {
            var last = _undoStack.FirstOrDefault();
            if (null != last)
                Undo(last);
        }

        public void Undo(ChangeSet value)
        {
            bool done = false;
            bool actionDone = false;
            _isWorking = true;
            try
            {
                do
                {
                    var changeSet = _undoStack.Pop();

                    if (changeSet == value || _undoStack.Count == 0)
                        done = true;

                    changeSet.Undo();
                    actionDone = true;

                    _redoStack.Push(changeSet);

                } while (!done);
            }
            finally
            {
                _isWorking = false;
            }
        }

        public bool CanRedo()
        {
            return _redoStack.Count > 0;
        }

        public void Redo()
        {
            var last = _redoStack.FirstOrDefault();
            if (null != last)
                Redo(last);
        }

        public void Redo(ChangeSet value)
        {
            bool done = false;
            bool actionDone = false;
            _isWorking = true;
            try
            {
                do
                {
                    var changeSet = _redoStack.Pop();

                    if (changeSet == value || _redoStack.Count == 0)
                        done = true;

                    changeSet.Redo();
                    actionDone = true;

                    _undoStack.Push(changeSet);

                } while (!done);
            }
            finally
            {
                _isWorking = false;
            }
        }

        private void AttachPropertyChanged(IList e)
        {
            foreach (INotifyPropertyChanged item in e)
                item.PropertyChanged += target_PropertyChanged;
        }

        private void DetachPropertyChanged(IList e)
        {
            foreach (INotifyPropertyChanged item in e)
                item.PropertyChanged -= target_PropertyChanged;
        }

        private void DetachCollectionChanged(IEnumerable<INotifyCollectionChanged> e)
        {
            foreach (INotifyCollectionChanged item in e)
                item.CollectionChanged -= collection_CollectionChanged;
        }

        void target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isWorking == true)
                return;

            PropertyChangedExtendedEventArgs newArgs = e as PropertyChangedExtendedEventArgs;
            if (newArgs != null)
            {
                _undoStack.Push(new ChangeSet(new Change(sender, newArgs), newArgs.Description));
            }
        }


        void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isWorking == true)
                return;

            if (e != null)
            {
                _undoStack.Push(new ChangeSet(new Change(sender, e), "Collection changed"));
            }

            if (e.OldItems != null)
            {
                DetachPropertyChanged(e.OldItems);
            }

            if (e.NewItems != null)
            {
                AttachPropertyChanged(e.NewItems);
            }
        }
    }
}
