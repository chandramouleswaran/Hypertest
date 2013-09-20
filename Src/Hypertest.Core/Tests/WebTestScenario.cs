using System;
using System.Runtime.Serialization;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// The basic unit of a web test scenario
    /// </summary>
    [DataContract]
    [Serializable]
    public class WebTestScenario : TestScenario
    {
        public WebTestScenario() : base()
        {
        }

        [DataMember]
        public string URL { get; internal set; }
    }
}
