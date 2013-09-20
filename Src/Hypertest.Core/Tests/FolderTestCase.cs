using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    [DataContract]
    public class FolderTestCase : TestCase
    {
        public FolderTestCase() : base()
        {
        }
    }
}
