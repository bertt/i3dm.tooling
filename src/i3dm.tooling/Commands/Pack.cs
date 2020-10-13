using System.IO;
using System;
using i3dm.tooling.Utils;
using I3dm.Tile;
using CommandLine;

namespace i3dm.tooling.Commands
{
    [Verb("pack", HelpText = "pack glb to i3dm")]
    public class Pack : ICommand
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the glb file")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, Default = "", HelpText = "Output path of the resulting .i3dm")]
        public string Output { get; set; }
        
        [Option('f', "force", Required = false, Default = false, HelpText = "force overwrite output file")]
        public bool Force { get; set; }

        public void Run()
        {
            Console.WriteLine($"Action: Pack");
            Console.WriteLine($"Input: {Input}");            
            var f = File.ReadAllBytes(Input);
            var batchTableJsonFile = Path.GetFileNameWithoutExtension(Input) + ".batch.csv";
            var featureTableJsonFile = Path.GetFileNameWithoutExtension(Input) + ".feature.csv";
            var positionsFile = Path.GetFileNameWithoutExtension(Input) + ".positions.csv";
            var normal_upsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".normal_ups.csv" : Output);
            var normal_rightsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".normal_rights.csv" : Output);
            var scale_non_uniformsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".scale_non_uniforms.csv" : Output);
            var scalesfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".scales.csv" : Output);

            var positions = FileUtils.ReadVectors(positionsFile);

            var i3dm = new I3dm.Tile.I3dm(positions, f);

            if (File.Exists(batchTableJsonFile))
            {
                Console.WriteLine($"Input batchtable json file: {batchTableJsonFile}");
                var batchTableJson = File.ReadAllText(batchTableJsonFile);
                i3dm.BatchTableJson = batchTableJson;
            }
            if (File.Exists(featureTableJsonFile))
            {
                Console.WriteLine($"Input featureTable json file: {featureTableJsonFile}");
                var featureTableJson = File.ReadAllText(featureTableJsonFile);
                i3dm.FeatureTableJson = featureTableJson;
            }
            if (File.Exists(normal_upsfile))
            {
                Console.WriteLine($"Input normal_upsfile file: {normal_upsfile}");
                var normal_ups = FileUtils.ReadVectors(normal_upsfile);
                i3dm.NormalUps = normal_ups;
            }
            if (File.Exists(normal_rightsfile))
            {
                Console.WriteLine($"Input normal_rightsfile file: {normal_rightsfile}");
                var normal_rights = FileUtils.ReadVectors(normal_rightsfile);
                i3dm.NormalRights = normal_rights;
            }
            if (File.Exists(scale_non_uniformsfile))
            {
                Console.WriteLine($"Input scale_non_uniforms file: {scale_non_uniformsfile}");
                var scale_non_uniforms = FileUtils.ReadVectors(scale_non_uniformsfile);
                i3dm.ScaleNonUniforms = scale_non_uniforms;
            }
            if (File.Exists(scalesfile))
            {
                Console.WriteLine($"Input scales file: {scalesfile}");
                var scales = FileUtils.ReadFloats(scalesfile);
                i3dm.Scales = scales;
            }

            var i3dmfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + "_new.i3dm" : Output);

            if (File.Exists(i3dmfile) && !Force)
            {
                Console.WriteLine($"File {i3dmfile} already exists. Specify -f or --force to overwrite existing files.");
            }
            else
            {
                I3dmWriter.Write(i3dmfile, i3dm);
                Console.WriteLine("I3dm created " + i3dmfile);
            }
        }
    }
}