using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// The basic unit of a test scenario in the Hypertest framework
    /// </summary>
    [DataContract]
    public abstract class TestScenario : FolderTestCase
    {
        protected TestScenario() : base()
        {

        }

        protected internal override ITestRegistry TestRegistry
        {
            get;
            set;
        }

        protected internal override ILoggerService LoggerService
        {
            get;
            set;
        }

        internal void SetDirty(bool p)
        {
            this.IsDirty = p;
        }

        internal void SetLocation(object info)
        {
            this.Location = info;
            this.IsDirty = false;
            RaisePropertyChanged("Location");
        }
    }
}
