using CommandLine;

namespace i3dm.tooling
{
    [Verb("info", HelpText = "info i3dm")]
    public class InfoOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the .i3dm")]
        public string Input { get; set; }
        [Option('b', "batchtablejson", Required = false, Default = false, HelpText = "display batchTableJSON")]
        public bool ShowBatchTableJson { get; set; }
    }

    [Verb("pack", HelpText = "pack glb to i3dm")]
    public class PackOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the glb file")]
        public string Input { get; set; }
        [Option('o', "output", Required = false, Default = "", HelpText = "Output path of the resulting .i3dm")]
        public string Output { get; set; }
        [Option('f', "force", Required = false, Default = false, HelpText = "force overwrite output file")]
        public bool Force { get; set; }
    }

    [Verb("unpack", HelpText = "unpack i3dm to glb")]
    public class UnpackOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the .i3dm")]
        public string Input { get; set; }
        [Option('o', "output", Required = false, Default = "", HelpText = "Output path of the resulting .glb")]
        public string Output { get; set; }
        [Option('f', "force", Required = false, Default = false, HelpText = "force overwrite output file")]
        public bool Force { get; set; }
    }
}
