using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Hypertest.Core.Tests
{
    [DataContract]
    public class FolderTestCase : TestCase
    {
        private ObservableCollection<TestCase> _children;

        public FolderTestCase() : base()
        {
            _children = new ObservableCollection<TestCase>();
        }

        [DataMember]
        public ObservableCollection<TestCase> Children
        {
            get { return _children; }
            set { _children = value; RaisePropertyChanged("Children"); }
        }

    }
}
