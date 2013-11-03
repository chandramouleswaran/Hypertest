﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Hypertest.Core.Attributes;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;
using Wide.Interfaces;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
    /// The basic unit of a test case in the Hypertest framework
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class TestCase : ContentModel, ICloneable, ICustomTypeDescriptor
    {
        #region Members
        private FolderTestCase _parent;
        private string _description;
        protected bool _isSelected;
        protected bool _isExpanded;
        protected TestCaseResult _expectedResult;
        protected TestCaseResult _actualResult;
        protected bool _markedForExecution;
        private ObservableCollection<Variable> _variables;
        private TestRunState _runState;
        #endregion

        #region CTOR
        protected TestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            if (_variables == null)
            {
                _variables = new ObservableCollection<Variable>();
            }
            _expectedResult = TestCaseResult.Passed;
            _runState = TestRunState.NotStarted;
        }
        #endregion


        #region Deserialize
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }

        #endregion

        #region Virtuals
        public virtual void Setup(){}
        public virtual void Body(){}
        public virtual void Cleanup(Exception e = null)
        {
            //Need to access logger
        }
        #endregion

        public void Run()
        {
            try
            {
                InitRun();
                Setup();
                RunState = TestRunState.Executing;
                Body();
                FinalizeRun();
                Cleanup();
            }
            catch (Exception e)
            {
                Cleanup(e);
            }
            finally
            {
                RunState = TestRunState.Done;
            }
        }

        protected internal TestCase ObjectToRun()
        {
            return this;
        }

        private void InitRun()
        {
            foreach (var variable in Variables)
            {
                
            }
        }
        
        private void FinalizeRun()
        {
            //This is where we want to look at the properties and assign it to variables
        }

        #region Properties
        [DataMember]
        [Description("Enter the description for the test case")]
        [Category("General")]
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
        [DisplayName("Expected Result")]
        [Description("The expected end result of the test case")]
        [Category("General")]
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
        [Category("General")]
        [Browsable(false)]
        public TestCaseResult ActualResult
        {
            get { return _actualResult; }
            set
            {
                _actualResult = value;
                RaisePropertyChanged();
            }
        }

        [DisplayName("Expected vs Actual Result")]
        [Description("The final result of the test case")]
        [Category("Results")]
        [DynamicBrowsable("RunState")]
        public TestCaseResult ExpectedVsActual
        {
            get
            {
                if (_expectedResult == TestCaseResult.None)
                    return TestCaseResult.Passed;
                else if (_expectedResult != _actualResult)
                    return TestCaseResult.Failed;
                else
                    return TestCaseResult.Passed;
            }
        }

        [DataMember]
        [NewItemTypes(typeof(Variable))]
        public virtual ObservableCollection<Variable> Variables
        {
            get { return _variables; }
            set
            {
                _variables = value;
                RaisePropertyChanged();
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
                if(oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _isExpanded, "Expand - " + _isExpanded.ToString());
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
        [Browsable(false),RefreshProperties(RefreshProperties.All)]
        public TestRunState RunState
        {
            get { return _runState; }
            internal set
            {
                _runState = value;
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
            if (this.TestRegistry != null)
            {
                var serializer = new DataContractSerializer(this.GetType(), this.TestRegistry.Tests);
                using (var ms = new System.IO.MemoryStream())
                {
                    serializer.WriteObject(ms, this);
                    ms.Position = 0;
                    return (TestCase) serializer.ReadObject(ms);
                }
            }
            else
            {
                throw new Exception("Test registry is null - please set the scenario's registry");
            }
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
            return this.GetProperties();
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
