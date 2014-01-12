#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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

        private readonly ObservableCollection<INotifyCollectionChanged> _collections;
        private readonly Stack<ChangeSet> _redoStack;
        private readonly ObservableCollection<INotifyPropertyChanged> _targets;
        private readonly Stack<ChangeSet> _undoStack;
        private int _batchCounter;
        private ChangeSet _currentBatch;
        private bool _isBatch;
        private bool _isWorking;
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
            if (_isWorking)
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
            ChangeSet last = _undoStack.FirstOrDefault();
            if (null != last)
                Undo(last);
        }

        public void Undo(ChangeSet value)
        {
            bool done = false;
            _isWorking = true;
            try
            {
                do
                {
                    ChangeSet changeSet = _undoStack.Pop();

                    if (changeSet == value || _undoStack.Count == 0)
                        done = true;

                    changeSet.Undo();

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
            ChangeSet last = _redoStack.FirstOrDefault();
            if (null != last)
                Redo(last);
        }

        public void Redo(ChangeSet value)
        {
            bool done = false;
            _isWorking = true;
            try
            {
                do
                {
                    ChangeSet changeSet = _redoStack.Pop();

                    if (changeSet == value || _redoStack.Count == 0)
                        done = true;

                    changeSet.Redo();

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
            if (_isWorking)
                return;

            var newArgs = e as PropertyChangedExtendedEventArgs;
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
            if (_isWorking)
                return;

            if (e != null)
            {
                if (_isBatch)
                {
                    _currentBatch.Changes.Add(new CollectionChange(sender, e));
                }
                else if (e.OldItems != null || e.NewItems != null) //If we have atleast something removed or added
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