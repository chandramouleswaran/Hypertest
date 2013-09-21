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

        [XmlIgnore]
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
        protected internal virtual ITestRegistry TestRegistry
        {
            get { return Scenario.TestRegistry; }
            set {}
        }

        [XmlIgnore]
        protected internal virtual ILoggerService LoggerService
        {
            get { return Scenario.LoggerService; }
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
    }
}
