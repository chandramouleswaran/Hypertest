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
using System.ComponentModel;
using System.Runtime.Serialization;
using Hypertest.Core.Attributes;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Loop")]
    [Description("Equivalent of a for loop")]
    [Category("General")]
    [TestImage("Images/Looper.png")]
    public class LooperTestCase : FolderTestCase
    {
        #region Members

        private int _counter;
        private int _loopCount;

        #endregion

        #region CTOR

        public LooperTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            this.Description = "Runs a loop";
            this.MarkedForExecution = true;
        }

        #endregion

        #region Property
        [DataMember]
        [DisplayName("Counter")]
        [Description("The property which holds the count at which the loop exits")]
        [DynamicReadonly("RunState"), DynamicBrowsable("RunState")]
        [Category("Settings")]
        [PostRun]
        public int Counter
        {
            get { return _counter; }
            set
            {
                int oldValue = _counter;
                _counter = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _counter, "Counter value change");
            }
        }

        [DataMember]
        [DisplayName("Loop Count")]
        [Description("The number of times you want to execute the children")]
        [DynamicReadonly("RunState")]
        [Category("Settings")]
        public int LoopCount
        {
            get { return _loopCount; }
            set
            {
                int oldValue = _loopCount;
                _loopCount = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _loopCount, "Loop count value change");
            }
        }

        #endregion

        #region Deserialize

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        #endregion

        #region Override
        protected override void Setup()
        {
            ObservableCollection<TestCase> newChildren = new ObservableCollection<TestCase>();
            for (int i = 0; i < this.LoopCount; i++)
            {
                LooperTestCase cloneTestCase = this.Clone() as LooperTestCase;
                FolderTestCase folder = new FolderTestCase();
                folder.Description = "For loop " + i;
                folder.Children = cloneTestCase.Children;
                newChildren.Add(folder);
            }

            this.Children = newChildren;
            //If you want to add break and if its a future requirement - override the Body specifically for a Break test case
        }
        #endregion
    }
}