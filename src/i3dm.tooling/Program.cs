using CommandLine;
using I3dm.Tile;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace i3dm.tooling
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<PackOptions, UnpackOptions, InfoOptions>(args).WithParsed(o =>
            {
                switch (o)
                {
                    case InfoOptions options:
                        Info(options);
                        break;
                    case PackOptions options:
                        Pack(options);
                        break;
                    case UnpackOptions options:
                        Unpack(options);
                        break;
                }
            });

        }

        private static void Pack(PackOptions options)
        {
            // todo: implement this one
            throw new NotImplementedException();
        }

        static void Info(InfoOptions o)
        {
            Console.WriteLine($"Action: Info");
            Console.WriteLine("i3dm file: " + o.Input);
            Console.WriteLine("Size: " + new FileInfo(o.Input).Length);
            var f = File.OpenRead(o.Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm header version: " + i3dm.I3dmHeader.Version);
            Console.WriteLine("i3dm GltfFormat: " + i3dm.I3dmHeader.GltfFormat);
            Console.WriteLine("i3dm header magic: " + i3dm.I3dmHeader.Magic);
            Console.WriteLine("i3dm featuretable json: '" + i3dm.FeatureTableJson + "'");
            Console.WriteLine("i3dm instances length: " + i3dm.FeatureTable.InstancesLength);
            if (o.ShowBatchTableJson)
            {
                Console.WriteLine("i3dm batch table json: '" + i3dm.BatchTableJson + "'");
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

        static void Unpack(UnpackOptions o)
        {
            Console.WriteLine($"Action: Unpack");
            Console.WriteLine($"Input: {o.Input}");
            var f = File.OpenRead(o.Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm version: " + i3dm.I3dmHeader.Version);
            var glbfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".glb" : o.Output);
            var batchfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".batch.csv" : o.Output);
            var positionsfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".positions.csv" : o.Output);
            var normal_upsfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".normal_ups.csv" : o.Output);
            var normal_rightsfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".normal_rights.csv" : o.Output);
            var scale_non_uniformsfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".scale__non_uniforms.csv" : o.Output);
            var scalesfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".scales.csv" : o.Output);

            if (File.Exists(glbfile) && !o.Force)
            {
                Console.WriteLine($"File {glbfile} already exists. Specify -f or --force to overwrite existing files.");
            }
            else
            {
                File.WriteAllBytes(glbfile, i3dm.GlbData);
                Console.WriteLine($"Glb created: {glbfile}");
                SaveItems(i3dm.Positions, positionsfile);
                Console.WriteLine($"Positions file created: {positionsfile}");

                if (i3dm.NormalUps != null)
                {
                    SaveItems(i3dm.NormalUps, normal_upsfile);
                    Console.WriteLine($"normalups file created: {normal_upsfile}");
                }
                if (i3dm.NormalRights != null)
                {
                    SaveItems(i3dm.NormalRights, normal_rightsfile);
                    Console.WriteLine($"normalrights file created: {normal_rightsfile}");
                }
                if (i3dm.ScaleNonUniforms!= null)
                {
                    SaveItems(i3dm.ScaleNonUniforms, scale_non_uniformsfile);
                    Console.WriteLine($"scale_non_uniforms file created: {scale_non_uniformsfile}");
                }
                if (i3dm.Scales != null)
                {
                    SaveItems(i3dm.Scales, scalesfile); ;
                    Console.WriteLine($"scales file created: {scalesfile}");
                }


                if (i3dm.BatchTableJson != String.Empty)
                {
                    var sb = new StringBuilder();
                    sb.Append(i3dm.FeatureTableJson);
                    sb.AppendLine();
                    sb.Append(i3dm.BatchTableJson);
                    File.WriteAllText(batchfile, sb.ToString());
                    Console.WriteLine($"batch file created: {batchfile}");
                }
            }
        }


        private static void SaveItems<T>(List<T> items, string filename)
        {
            var w = new StreamWriter(filename);

            foreach (var item in items)
            {
                var s = item.ToString();
                s= s.Trim('<').Trim('>');
                w.WriteLine(s);
            }
            w.Flush();
        }


        private static void PrintItems<T>(List<T> items, string name)
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
