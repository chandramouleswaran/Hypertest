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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;
using Hypertest.Core.Attributes;
using Hypertest.Core.Interfaces;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    public enum TestCaseResult
    {
        None,
        Passed,
        Failed
    }

    public enum TestRunState
    {
        NotStarted = 0,
        Executing = 1,
        Done = 2
    }

    /// <summary>
    ///     The basic unit of a test case in the Hypertest framework
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class TestCase : ContentModel, ICloneable, ICustomTypeDescriptor
    {
        #region Members
        protected TestCaseResult _actualResult;
        private string _description;
        private int _waitTime;
        protected TestCaseResult _expectedResult;
        protected bool _isExpanded;
        protected bool _isSelected;
        protected bool _markedForExecution;
        private FolderTestCase _parent;
        private TestRunState _runState;
        private ObservableCollection<PostRunPairs> _postValues;
        private ObservableCollection<string> _logMessages;
        #endregion

        #region CTOR

        protected TestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            if (create)
            {
                _expectedResult = TestCaseResult.Passed;
                _runState = TestRunState.NotStarted;
                this.LogMessages = new ObservableCollection<string>();
                _postValues = new ObservableCollection<PostRunPairs>();
            }
        }
        #endregion

        #region Deserialize
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize(false);
        }

        #endregion

        #region Virtuals
        protected virtual void Setup()
        {
        }

        protected virtual void Body()
        {
        }

        protected virtual void Wait()
        {
            Thread.Sleep(this.WaitTime);
        }

        protected virtual void Cleanup(Exception ex = null)
        {
            if (ex != null)
            {
                this.Log(ex.Message, LogCategory.Exception, LogPriority.High);
                this.Log(ex.StackTrace, LogCategory.Exception, LogPriority.High);
            }
        }
        #endregion

        #region Methods
        public void Run()
        {
            try
            {
                this.LogMessages.Clear();
                this.Log("Run Started", LogCategory.Info, LogPriority.None);
                Setup();
                Dispatcher.CurrentDispatcher.Invoke(() => RunState = TestRunState.Executing);
                Body();
                Wait();
                FinalizeRun();
                Cleanup();
            }
            catch (Exception ex)
            {
                this.ActualResult = TestCaseResult.Failed;
                Cleanup(ex);
            }
            finally
            {
                Dispatcher.CurrentDispatcher.Invoke(() => RunState = TestRunState.Done);
                this.Log("Run Ended", LogCategory.Info, LogPriority.None);
            }
        }

        private void FinalizeRun()
        {
            //Reflect the property and store the value
            foreach (PostRunPairs p in PostValues.Where(f => !string.IsNullOrEmpty(f.VariableName)))
            {
                PropertyInfo property = this.GetType().GetProperties().First(prop => prop.IsDefined(typeof(PostRunAttribute), false) && prop.Name.Equals(p.PropertyName));
                if (property != null)
                {
                    try
                    {
                        Object val = property.GetValue(this, null);
                        if (val != null)
                        {
                            Variable v = new Variable(p.VariableName, val);
                            this.Runner.AddVariable(v);
                            this.Log(v.ToString(), LogCategory.Info, LogPriority.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Log(ex.Message, LogCategory.Exception, LogPriority.High);
                        this.Log(ex.StackTrace, LogCategory.Exception, LogPriority.High);
                    }
                }
            }
        }

        protected internal void Log(string message, LogCategory category, LogPriority priority)
        {
            this.LoggerService.Log(string.Format("[{0}] {1}", this.Description, message), category, priority);
            this.LogMessages.Add(string.Format("[{0}] {1}", DateTime.Now.ToString(), message));
			this.RaisePropertyChanged("LogMessages");
        }
        #endregion

        #region Properties
        [DataMember]
        [Description("Enter the description for the test case")]
        [Category("General")]
        [DynamicReadonly("RunState")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    string oldValue = _description;
                    _description = value;
                    if (oldValue != value)
                        RaisePropertyChangedWithValues(oldValue, _description, "Description change");
                }
            }
        }

        [DataMember]
        [DisplayName("Wait Time")]
        [Description("Enter the time to wait in milli seconds before executing the next step")]
        [Category("General")]
        [DefaultValue(0)]
        [DynamicReadonly("RunState")]
        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                if (value != _waitTime)
                {
                    int oldValue = _waitTime;
                    _waitTime = value;
                    if (oldValue != value)
                        RaisePropertyChangedWithValues(oldValue, _waitTime, "Wait change");
                }
            }
        }

        [DataMember]
        [DisplayName("Expected Result")]
        [Description("The expected end result of the test case")]
        [Category("Results")]
        [DynamicReadonly("RunState")]
        public TestCaseResult ExpectedResult
        {
            get { return _expectedResult; }
            set
            {
                _expectedResult = value;
                RaisePropertyChanged();
            }
        }

        [DataMember]
        [DisplayName("Actual Result")]
        [Description("The actual end result of the test case")]
        [Category("Results")]
        [DynamicBrowsable("RunState"), DynamicReadonly("RunState")]
        public TestCaseResult ActualResult
        {
            get { return _actualResult; }
            set
            {
                _actualResult = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ExpectedVsActual");
            }
        }

        [DisplayName("Expected vs Actual Result")]
        [Description("The final result of the test case")]
        [Category("Results")]
        [DynamicBrowsable("RunState"), DynamicReadonly("RunState")]
        public TestCaseResult ExpectedVsActual
        {
            get
            {
                if (_expectedResult == TestCaseResult.None)
                    return TestCaseResult.Passed;
                if (_expectedResult != _actualResult)
                    return TestCaseResult.Failed;
                return TestCaseResult.Passed;
            }
        }


        [XmlIgnore]
        [Browsable(false)]
        public FolderTestCase Parent
        {
            get { return _parent; }
            internal set
            {
                _parent = value;
                RaisePropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            internal set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public virtual bool IsExpanded
        {
            get { return _isExpanded; }
            internal set
            {
                bool oldValue = _isExpanded;
                _isExpanded = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _isExpanded, "Expand - " + _isExpanded);
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public TestScenario Scenario
        {
            get
            {
                TestCase test = this;
                while (test.Parent != null)
                {
                    test = test.Parent;
                }
                return (TestScenario) test;
            }
        }

        [Browsable(false)]
        public bool MarkedForExecution
        {
            get { return _markedForExecution; }
            set
            {
                _markedForExecution = value;
                RaisePropertyChanged();
            }
        }

        [DataMember]
        [Browsable(false), RefreshProperties(RefreshProperties.All)]
        [DefaultValue(TestRunState.NotStarted)]
        public TestRunState RunState
        {
            get { return _runState; }
            internal set
            {
                _runState = value;
                RaisePropertyChanged();
            }
        }

        [DataMember]
        [Browsable(false)]
        public ObservableCollection<PostRunPairs> PostValues
        {
            get { return _postValues; }
            set
            {
                _postValues = value;
                RaisePropertyChanged();
            }
        }
        
        [DataMember]
        [Browsable(false)]
        public ObservableCollection<string> LogMessages
        {
            get { return _logMessages; }
            internal set
            {
                _logMessages = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Internal Properties

        [XmlIgnore]
        [Browsable(false)]
        protected internal virtual ITestRegistry TestRegistry
        {
            get { return Scenario.TestRegistry; }
            set { }
        }

        [XmlIgnore]
        [Browsable(false)]
        protected internal virtual ILoggerService LoggerService
        {
            get { return Scenario.LoggerService; }
            set { }
        }

        [XmlIgnore]
        [Browsable(false)]
        protected internal virtual IRunner Runner
        {
            get { return Scenario.Runner; }
            set { }
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            if (TestRegistry != null)
            {
                var serializer = new DataContractSerializer(GetType(), TestRegistry.Tests);
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, this);
                    ms.Position = 0;
                    var value = serializer.ReadObject(ms) as TestCase;
                    var wts = value as WebTestScenario;
                    if (wts != null)
                    {
                        wts.TestRegistry = this.TestRegistry;
                        wts.LoggerService = this.LoggerService;
                        wts.Runner = this.Runner;
                    }
                    return value;
                }
            }
            throw new Exception("Test registry is null - please set the scenario's registry");
        }

        #endregion

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return DynamicTypeDescriptor.GetProperties(this);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}