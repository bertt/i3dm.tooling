using System;
using System.IO;
using i3dm.tooling.Utils;
using I3dm.Tile;
using CommandLine;

namespace i3dm.tooling.Commands
{
    [Verb("unpack", HelpText = "unpack i3dm to glb")]
    public class Unpack : ICommand
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the .i3dm")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, Default = "", HelpText = "Output path of the resulting .glb")]
        public string Output { get; set; }

        [Option('f', "force", Required = false, Default = false, HelpText = "force overwrite output file")]
        public bool Force { get; set; }
    
        public void Run()
        {
            Console.WriteLine($"Action: Unpack");
            Console.WriteLine($"Input: {Input}");
            var f = File.OpenRead(Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm version: " + i3dm.I3dmHeader.Version);
            var glbfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".glb" : Output);
            var batchfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".batch.csv" : Output);
            var featurefile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".feature.csv" : Output);
            var positionsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".positions.csv" : Output);
            var normal_upsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".normal_ups.csv" : Output);
            var normal_rightsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".normal_rights.csv" : Output);
            var scale_non_uniformsfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".scale_non_uniforms.csv" : Output);
            var scalesfile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + ".scales.csv" : Output);

            if (File.Exists(glbfile) && !Force)
            {
                Console.WriteLine($"File {glbfile} already exists. Specify -f or --force to overwrite existing files.");
            }
            else
            {
                File.WriteAllBytes(glbfile, i3dm.GlbData);
                Console.WriteLine($"Glb created: {glbfile}");
                FileUtils.SaveItems(i3dm.Positions, positionsfile);
                Console.WriteLine($"Positions file created: {positionsfile}");

                if (i3dm.NormalUps != null)
                {
                    FileUtils.SaveItems(i3dm.NormalUps, normal_upsfile);
                    Console.WriteLine($"normalups file created: {normal_upsfile}");
                }
                if (i3dm.NormalRights != null)
                {
                    FileUtils.SaveItems(i3dm.NormalRights, normal_rightsfile);
                    Console.WriteLine($"normalrights file created: {normal_rightsfile}");
                }
                if (i3dm.ScaleNonUniforms != null)
                {
                    FileUtils.SaveItems(i3dm.ScaleNonUniforms, scale_non_uniformsfile);
                    Console.WriteLine($"scale_non_uniforms file created: {scale_non_uniformsfile}");
                }
                if (i3dm.Scales != null)
                {
                    FileUtils.SaveItems(i3dm.Scales, scalesfile); ;
                    Console.WriteLine($"scales file created: {scalesfile}");
                }

                if (i3dm.BatchTableJson != String.Empty)
                {
                    File.WriteAllText(batchfile, i3dm.BatchTableJson);
                    Console.WriteLine($"batch file created: {batchfile}");
                }
                if (i3dm.FeatureTableJson != String.Empty)
                {
                    File.WriteAllText(featurefile, i3dm.FeatureTableJson);
                    Console.WriteLine($"feature file created: {featurefile}");
                }
            }
        }
    }
}