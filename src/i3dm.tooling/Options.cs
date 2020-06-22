using CommandLine;

namespace i3dm.tooling
{
    [Verb("info", HelpText = "info b3dm")]
    public class InfoOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the .i3dm")]
        public string Input { get; set; }
    }

}
