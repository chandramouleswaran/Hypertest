using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Folder")]
    [Description("A test case which holds a list of test cases")]
    [Category("General")]
    public class FolderTestCase : TestCase
    {
        #region Members
        protected ObservableCollection<TestCase> _children;
        #endregion

        #region CTOR
        public FolderTestCase() : base()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            if (_children == null)
            {
                _children = new ObservableCollection<TestCase>();
            }
        }
        #endregion

        #region Property
        [DataMember]
        [Browsable(false)]
        public ObservableCollection<TestCase> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                RaisePropertyChanged("Children");
            }
        }
        #endregion

        #region Deserialize

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

        #endregion

        #region Events
        //USED ONLY TO SET THE PARENT - NOTHING TO DO WITH STATE MANAGER.
        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TestCase test in e.NewItems)
                {
                    test.Parent = this;
                }
            }
        }

        #endregion
    }
}
