using CommandLine;

namespace Hypertest
{
    class Options
    {
        [Value(0, DefaultValue = "")]
        public string OpenFile { get; set; }


    }
}
