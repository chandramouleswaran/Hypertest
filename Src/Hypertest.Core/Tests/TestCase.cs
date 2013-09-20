using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;
using Wide.Interfaces;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// The basic unit of a test case in the Hypertest framework
    /// </summary>
    [DataContract]
    public abstract class TestCase : ContentModel, ICloneable
    {
        #region Members
        private TestCase _parent;
        private string _description;
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

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    string oldValue = _description;
                    _description = value;
                    RaisePropertyChangedWithUndo(oldValue, _description, "Description change");
                }
            }
        }

        public TestCase Parent
        {
            get { return _parent; }
            internal set
            {
                _parent = value;
                RaisePropertyChanged();
            }
        }

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

        protected internal virtual ITestRegistry TestRegistry
        {
            get { return Scenario.TestRegistry; }
            set {}
        }

        protected internal virtual ILoggerService LoggerService
        {
            get { return Scenario.LoggerService; }
            set { }
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChangedWithUndo(object oldValue, object newValue, string description, [CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedExtendedEventArgs(propertyName, oldValue, newValue, description));
            }
        }

        #region ICloneable
        public object Clone()
        {       
            var serializer = new DataContractSerializer(this.GetType(), this.TestRegistry.Tests);
            using (var ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, this);
                ms.Position = 0;
                return (TestCase)serializer.ReadObject(ms);
            }
        }
        #endregion
    }
}
