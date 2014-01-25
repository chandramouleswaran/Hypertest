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
using Hypertest.Core.Interfaces;
using Hypertest.Core.Runners;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Variable")]
    [Description("Create and set initial values for variables")]
    [Category("General")]
    [TestImage("Images/SetVariable.png")]
    public class SetVariableTestCase : TestCase
    {
        #region Members

        private ObservableCollection<Variable> _variables;

        #endregion

        #region CTOR

        public SetVariableTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            if (_variables == null)
            {
                _variables = new ObservableCollection<Variable>();
            }
            this.Description = "Create and set initial variables";
            this.MarkedForExecution = true;
        }

        #endregion

        #region Property

        [DataMember]
        [DisplayName("Variables")]
        [Description("Click to create and initialize variables")]
        [NewItemTypes(typeof (Variable))]
        [Category("Settings")]
        public virtual ObservableCollection<Variable> Variables
        {
            get { return _variables; }
            set
            {
                _variables = value;
                RaisePropertyChanged();
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

        public override void Setup()
        {
            foreach (Variable variable in Variables)
            {
                WebScenarioRunner.Current.AddVariable(variable);
            }
        }

        public override void Body()
        {
            this.ActualResult = TestCaseResult.Passed;
        }

        #endregion
    }
}