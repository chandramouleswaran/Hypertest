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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using Hypertest.Core.Attributes;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Folder")]
    [Description("A test case which holds a list of test cases")]
    [Category("General")]
    [TestImage("Images/Folder.png")]
    public class FolderTestCase : TestCase
    {
        #region Members

        protected ObservableCollection<TestCase> _children;
        protected bool _exitTotally;

        #endregion

        #region CTOR

        public FolderTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            if (_children == null)
            {
                _children = new ObservableCollection<TestCase>();
            }
            this.Description = "Folder Test Case";
            this.MarkedForExecution = true;
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
                var old = _children;
                _children = value;
                if (_children != old)
                {
                    _children.CollectionChanged += _children_CollectionChanged;
                    foreach (var item in _children)
                    {
                        item.Parent = this;
                    }
                    RaisePropertyChanged("Children");
                }
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

        #region Override
        protected override void Body()
        {
            this.ActualResult = TestCaseResult.Passed;
            foreach (TestCase child in _children)
            {
                if (child.MarkedForExecution)
                {
                    child.Run();
                    if (child.ExpectedVsActual == TestCaseResult.Failed)
                        this.ActualResult = TestCaseResult.Failed;
                }
            }
        }
        #endregion

        #region Virtuals
        public virtual bool AreNewItemsAllowed()
        {
            return true;
        }
        #endregion
    }
}