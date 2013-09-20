using System.Runtime.Serialization;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// The basic unit of a web test scenario
    /// </summary>
    [DataContract]
    public class WebTestScenario : TestScenario
    {
        public WebTestScenario() : base()
        {
            this.URL = "http://www.google.com";
            this.Description = "Web test scenario root !!";
            this.Children.Add(new FolderTestCase() {Description = "Folder 1"});
            this.Children.Add(new FolderTestCase() { Description = "Folder 2" });
        }

        [DataMember]
        public string URL { get; internal set; }
    }
}
