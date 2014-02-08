using CommandLine;
using Hypertest.Core.Runners;

namespace Hypertest
{
    class Options
    {
        [Value(0)]
        public string OpenFile { get; set; }
        
        [Option('b', "browser", HelpText = "Indicate the browser here. InternetExplorer, Chrome, Firefox")]
        public string Browser { get; set; }
    }
}
