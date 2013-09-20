using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;
using System;
using Hypertest.Core.Manager;
using System.Collections.Specialized;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// The basic unit of a test scenario in the Hypertest framework
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class TestScenario : FolderTestCase
    {
        #region Member
        protected StateManager _manager;
        #endregion

        #region CTOR and other initializers
        protected TestScenario() : base()
        {
            Initialize();
            _children.CollectionChanged += _children_CollectionChanged;
            BulkMonitor(this);
            this.IsSelected = true;
            this.IsExpanded = true;
            _manager.Clear();
        }

        private void Initialize()
        {
            _manager = new StateManager();
        }
        #endregion

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize();
            foreach (TestCase test in _children)
            {
                test.Parent = this;
            }
            _children.CollectionChanged += _children_CollectionChanged;
            BulkMonitor(this);
            this.IsSelected = true;
            this.IsExpanded = true;
            _manager.Clear();
        }

        protected void BulkMonitor(TestCase testCase)
        {
            _manager.MonitorObject(testCase);
            FolderTestCase ftc = testCase as FolderTestCase;
            if (ftc != null)
            {
                ftc.Children.CollectionChanged += _children_CollectionChanged;
                _manager.MonitorCollection(ftc.Children);
                foreach (TestCase tc in ftc.Children)
                {
                    BulkMonitor(tc);
                }
            }
        }

        protected void BulkUnmonitor(TestCase testCase)
        {
            _manager.UnmonitorObject(testCase);
            FolderTestCase ftc = testCase as FolderTestCase;
            if (ftc != null)
            {
                ftc.Children.CollectionChanged += _children_CollectionChanged;
                _manager.UnmonitorCollection(ftc.Children);
                foreach (TestCase tc in ftc.Children)
                {
                    BulkUnmonitor(tc);
                }
            }
        }

        void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TestCase test in e.NewItems)
                {
                    test.Parent = this;
                }
            }
            
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this._manager != null && e.NewItems != null)
                {
                    foreach (TestCase test in e.NewItems)
                    {
                        BulkMonitor(test);
                    }
                }
            }

            if (this._manager != null && e.OldItems != null)
            {
                foreach (TestCase test in e.OldItems)
                {
                    BulkUnmonitor(test);
                }
            }
        }

        internal void SetDirty(bool p)
        {
            this.IsDirty = p;
        }

        internal void SetLocation(object info)
        {
            this.Location = info;
            this.IsDirty = false;
            RaisePropertyChanged("Location");
        }

        #region Property
        protected internal override ITestRegistry TestRegistry
        {
            get;
            set;
        }

        protected internal StateManager Manager { get { return _manager; } }

        protected internal override ILoggerService LoggerService
        {
            get;
            set;
        }
        #endregion
    }
}
