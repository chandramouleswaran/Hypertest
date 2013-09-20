using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    public class FolderTestCase : TestCase
    {
        protected ObservableCollection<TestCase> _children;

        public FolderTestCase() : base()
        {
            Initialize();
        }

        [DataMember]
        public ObservableCollection<TestCase> Children
        {
            get { return _children; }
            set 
            {
                _children = value;
                RaisePropertyChanged("Children"); 
            }
        }

        private void Initialize(bool create = true)
        {
            if (_children == null)
            {
                _children = new ObservableCollection<TestCase>();
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (TestCase test in _children)
            {
                test.Parent = this;
            }
            _children.CollectionChanged += _children_CollectionChanged;
        }

        //USED ONLY TO SET THE PARENT - NOTHING TO DO WITH STATE MANAGER.
        void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TestCase test in e.NewItems)
                {
                    test.Parent = this;
                }
            }
        }

    }
}
