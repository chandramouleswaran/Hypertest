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
        private string _url;

        public WebTestScenario() : base()
        {
        }

        [DataMember]
        public string URL
        {
            get { return _url; }
            set
            {
                string oldValue = _url;
                if (oldValue != value)
                {
                    _url = value;
                    RaisePropertyChangedWithValues(oldValue,value,"URL change");
                }
            }
        }
    }
}
