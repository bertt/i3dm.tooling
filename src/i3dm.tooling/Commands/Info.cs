using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I3dm.Tile;
using SharpGLTF.Schema2;
using CommandLine;

namespace i3dm.tooling.Commands
{
    [Verb("info", HelpText = "info i3dm")]
    public class Info : ICommand
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the .i3dm")]
        public string Input { get; set; }

        [Option('b', "batchtablejson", Required = false, Default = false, HelpText = "display batchTableJSON")]
        public bool ShowBatchTableJson { get; set; }

        public void Run()
        {
            Console.WriteLine($"Action: Info");
            Console.WriteLine("i3dm file: " + Input);
            Console.WriteLine("Size: " + new FileInfo(Input).Length);
            var f = File.OpenRead(Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm header version: " + i3dm.I3dmHeader.Version);
            Console.WriteLine("i3dm GltfFormat: " + i3dm.I3dmHeader.GltfFormat);
            Console.WriteLine("i3dm header magic: " + i3dm.I3dmHeader.Magic);
            Console.WriteLine("i3dm featuretable json: '" + i3dm.FeatureTableJson + "'");
            Console.WriteLine("i3dm instances length: " + i3dm.FeatureTable.InstancesLength);
            if (ShowBatchTableJson)
            {
                Console.WriteLine("i3dm batch table json: '" + i3dm.BatchTableJson + "'");
            }
            else
            {
                Console.WriteLine("i3dm batch table json not shown (use ShowBatchTableJson)");
            }
            if (i3dm.FeatureTable.BatchIdOffset != null && i3dm.FeatureTable.BatchIdOffset.componentType != null)
            {
                Console.WriteLine("i3dm batchId component type: " + i3dm.FeatureTable.BatchIdOffset.componentType);
            }
            if (i3dm.FeatureTable.RtcCenter != null)
            {
                Console.WriteLine("i3dm RTC_CENTER: " + i3dm.FeatureTable.RtcCenter);
            }
            var validationErrors = i3dm.I3dmHeader.Validate();
            if (validationErrors.Count > 0)
            {
                Console.WriteLine($"Validation check: {validationErrors.Count} errors");
                foreach (var error in validationErrors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.WriteLine("Validation check: no errors");
            }

            PrintItems(i3dm.Positions, "positions ");
            PrintItems(i3dm.NormalUps, "normal ups ");
            PrintItems(i3dm.NormalRights, "normal rights ");
            PrintItems(i3dm.ScaleNonUniforms, "Scale non-uniform ");
            PrintItems(i3dm.Scales, "Scales");

            PrintItems(i3dm.BatchIds, "Batch ids: ");

            var stream = new MemoryStream(i3dm.GlbData);
            try
            {
                var glb = ModelRoot.ReadGLB(stream);
                Console.WriteLine("glTF model is loaded");
                Console.WriteLine("glTF generator: " + glb.Asset.Generator);
                Console.WriteLine("glTF version:" + glb.Asset.Version);
                Console.WriteLine("glTF primitives: " + glb.LogicalMeshes[0].Primitives.Count);
                var triangles = Toolkit.EvaluateTriangles(glb.DefaultScene).ToList();
                Console.WriteLine("glTF triangles: " + triangles.Count);

                var points = triangles.SelectMany(item => new[] { item.A.GetGeometry().GetPosition(), item.B.GetGeometry().GetPosition(), item.C.GetGeometry().GetPosition() }.Distinct().ToList());
                var xmin = (from p in points select p.X).Min();
                var xmax = (from p in points select p.X).Max();
                var ymin = (from p in points select p.Y).Min();
                var ymax = (from p in points select p.Y).Max();
                var zmin = (from p in points select p.Z).Min();
                var zmax = (from p in points select p.Z).Max();

                Console.WriteLine($"Bounding box vertices (xmin, ymin, zmin, xmax, ymax, zmax): {xmin}, {ymin}, {zmin}, {xmax}, {ymax}, {zmax}");

                if (glb.ExtensionsUsed != null)
                {
                    Console.WriteLine("glTF extensions used:" + string.Join(',', glb.ExtensionsUsed));
                }
                else
                {
                    Console.WriteLine("glTF: no extensions used.");
                }
                if (glb.ExtensionsRequired != null)
                {
                    Console.WriteLine("glTF extensions required:" + string.Join(',', glb.ExtensionsRequired));
                }
                else
                {
                    Console.WriteLine("glTF: no extensions required.");
                }

                if (glb.LogicalMeshes[0].Primitives.Count > 0)
                {
                    Console.WriteLine("glTF primitive mode: " + glb.LogicalMeshes[0].Primitives[0].DrawPrimitiveType);
                }
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine("glTF version not supported.");
                Console.WriteLine(ex.Message);
            }
            f.Dispose();
        }

        public static void PrintItems<T>(List<T> items, string name)
        {
            if (items != null)
            {
                var extra = items.Count > 5 ? "(first 5)" : String.Empty;
                var firstfive = items.Take(5);
                Console.WriteLine(name + extra + ": " + string.Join(',', firstfive));
            }
            else
            {
                Console.WriteLine(name + ": -");
            }
        }
    }
}