using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;
using Wide.Interfaces;
using System.Xml.Serialization;

namespace Hypertest.Core.Tests
{

    public enum TestCaseResult
    {
        None,
        Passed,
        Failed
    }

    /// <summary>
    /// The basic unit of a test case in the Hypertest framework
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class TestCase : ContentModel, ICloneable
    {
        #region Members
        private FolderTestCase _parent;
        private string _description;
        protected bool _isSelected;
        protected bool _isExpanded;
        protected TestCaseResult _expectedResult;
        protected TestCaseResult _actualResult;
        protected bool _markedForExecution;
        #endregion

        #region CTOR
        protected TestCase()
        {
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
                Body();
                FinalizeRun();
                Cleanup();
            }
            catch (Exception e)
            {
                Cleanup(e);
            }
        }

        protected internal TestCase ObjectToRun()
        {
            return this;
        }

        private void InitRun()
        {
            //Set the actual result to None
        }
        
        private void FinalizeRun()
        {
            //This is where we want to look at the properties and assign it to variables
        }

        #region Properties
        [DataMember]
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

        protected internal virtual TestCaseResult ExpectedVsActual
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
                TestCase result = this;
                while (result.Parent != null)
                {
                    result = result.Parent;
                }
                return (TestScenario) result;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        protected internal virtual ITestRegistry TestRegistry
        {
            get { return Scenario.TestRegistry; }
            set {}
        }

        [XmlIgnore]
        [Browsable(false)]
        protected internal virtual ILoggerService LoggerService
        {
            get { return Scenario.LoggerService; }
            set { }
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
    }
}
