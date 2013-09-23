using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Wide.Utils;

namespace Hypertest.Core.Manager
{
    public class StateManager
    {

        #region Members
        private Stack<ChangeSet> _undoStack;
        private Stack<ChangeSet> _redoStack;
        private bool _isWorking;
        private bool _isBatch;
        private int _batchCounter = 0;
        private ChangeSet _currentBatch;
        private ObservableCollection<INotifyPropertyChanged> _targets;
        private ObservableCollection<INotifyCollectionChanged> _collections;
        public event EventHandler StateChange;
        #endregion

        #region CTOR
        public StateManager()
        {
            _undoStack = new Stack<ChangeSet>();
            _redoStack = new Stack<ChangeSet>();
            _isWorking = false;
            _targets = new ObservableCollection<INotifyPropertyChanged>();
            _collections = new ObservableCollection<INotifyCollectionChanged>();
        }
        #endregion

        #region Property
        public IEnumerable<ChangeSet> UndoStack
        {
            get { return _undoStack; }
        }

        public IEnumerable<ChangeSet> RedoStack
        {
            get { return _redoStack; }
        }

        public bool IsBatch
        {
            get { return _isBatch; }
        }
        #endregion

        #region Batch

        public void BeginChangeSetBatch(string batchDescription)
        {
            if (_isBatch)
                return;

            _batchCounter++;
            _isBatch = true;

            if (_batchCounter == 1)
            {
                _currentBatch = new ChangeSet(batchDescription);
                _undoStack.Push(_currentBatch);
            }
        }

        public void EndChangeSetBatch()
        {
            _batchCounter--;

            if (_batchCounter < 0)
                _batchCounter = 0;

            if (_batchCounter == 0)
            {
                _currentBatch = null;
                _isBatch = false;
            }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public void Dispose()
        {
            DetachCollectionChanged(_collections);
            foreach (INotifyPropertyChanged target in _targets)
            {
                UnmonitorObject(target);
            }
        }


        private void DetachCollectionChanged(IList e)
        {
            foreach (INotifyCollectionChanged item in e)
                item.CollectionChanged -= collection_CollectionChanged;
        }

        public void AddChange(Change change, string description)
        {
            if (_isWorking == true)
                return;
            if (_isBatch)
            {
                _currentBatch.Changes.Add(change);
            }
            else
            {
                _undoStack.Push(new ChangeSet(change, description));
                RaiseStateChangeEvent();
            }
            _redoStack.Clear();
        }

        #endregion

        #region Monitor & Unmonitor

        public void MonitorCollection(INotifyCollectionChanged collection)
        {
            if (collection != null && !_collections.Contains(collection))
            {
                _collections.Add(collection);
                collection.CollectionChanged += collection_CollectionChanged;
            }
        }

        public void UnmonitorCollection(INotifyCollectionChanged collection)
        {
            if (_collections.Contains(collection))
            {
                _collections.Remove(collection);
                collection.CollectionChanged -= collection_CollectionChanged;
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

        public void UnmonitorObject(INotifyPropertyChanged target)
        {
            if (_targets.Contains(target))
            {
                _targets.Remove(target);
                target.PropertyChanged -= target_PropertyChanged;
            }
        }

        #endregion

        #region Undo & Redo

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
                    RaiseStateChangeEvent();

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
                    RaiseStateChangeEvent();

                } while (!done);
            }
            finally
            {
                _isWorking = false;
            }
        }

        #endregion

        #region Events

        private void target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isWorking == true)
                return;

            PropertyChangedExtendedEventArgs newArgs = e as PropertyChangedExtendedEventArgs;
            if (newArgs != null)
            {
                if (_isBatch)
                {
                    _currentBatch.Changes.Add(new PropertyChange(sender, newArgs));
                    RaiseStateChangeEvent();
                }
                else
                {
                    _undoStack.Push(new ChangeSet(new PropertyChange(sender, newArgs), newArgs.Description));
                    RaiseStateChangeEvent();
                }
                _redoStack.Clear();
            }
        }


        private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isWorking == true)
                return;

            if (e != null)
            {
                if (_isBatch)
                {
                    _currentBatch.Changes.Add(new CollectionChange(sender, e));
                }
                else
                {
                    _undoStack.Push(new ChangeSet(new CollectionChange(sender, e), "Collection changed"));
                    RaiseStateChangeEvent();
                }
                _redoStack.Clear();
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    UnmonitorObject(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    MonitorObject(item);
                }
            }
        }

        private void RaiseStateChangeEvent()
        {
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        #endregion

    }
}
