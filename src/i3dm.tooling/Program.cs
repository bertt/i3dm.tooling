using CommandLine;
using I3dm.Tile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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
            var f = File.OpenRead(o.Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm header version: " + i3dm.I3dmHeader.Version);
            Console.WriteLine("i3dm GltfFormat: " + i3dm.I3dmHeader.GltfFormat);
            Console.WriteLine("i3dm header magic: " + i3dm.I3dmHeader.Magic);
            Console.WriteLine("i3dm featuretable json: " + i3dm.FeatureTableJson);
            Console.WriteLine("i3dm instances length: " + i3dm.FeatureTable.InstancesLength);
            Console.WriteLine("i3dm batch table json: " + i3dm.BatchTableJson);
            if(i3dm.FeatureTable.BatchIdOffset!=null && i3dm.FeatureTable.BatchIdOffset.componentType != null)
            {
                Console.WriteLine("i3dm batchId component type: " + i3dm.FeatureTable.BatchIdOffset.componentType);
            }

            if (i3dm.RtcCenter != Vector3.Zero)
            {
                Console.WriteLine("i3dm rtc_center: " + i3dm.RtcCenter);
            }

            PrintItems<Vector3>(i3dm.Positions, "positions: ");
            PrintItems<Vector3>(i3dm.NormalUps, "normal ups: ");
            PrintItems<Vector3>(i3dm.NormalRights, "normal rights: ");
            PrintItems<Vector3>(i3dm.ScaleNonUniforms, "Scale non-uniform: ");
            PrintItems<float>(i3dm.Scales, "Scales");

            PrintItems<int>(i3dm.BatchIds, "Batch ids: ");

            var stream = new MemoryStream(i3dm.GlbData);
            try
            {
                var glb = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream);
                Console.WriteLine("glTF model is loaded");
                Console.WriteLine("glTF generator: " + glb.Asset.Generator);
                Console.WriteLine("glTF version:" + glb.Asset.Version);
                Console.WriteLine("glTF primitives: " + glb.LogicalMeshes[0].Primitives.Count);
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

        private static void PrintItems<T>(List<T> items, string name)
        {
            if (items != null)
            {
                Console.WriteLine(name + string.Join(',', items));
            }
            else
            {
                Console.WriteLine(name + ": -");
            }
        }

        static void Unpack(UnpackOptions o)
        {
            Console.WriteLine($"Action: Unpack");
            Console.WriteLine($"Input: {o.Input}");
            var f = File.OpenRead(o.Input);
            var i3dm = I3dmReader.Read(f);
            Console.WriteLine("i3dm version: " + i3dm.I3dmHeader.Version);
            var stream = new MemoryStream(i3dm.GlbData);
            try
            {
                var gltf = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream);

                Console.WriteLine("glTF asset generator: " + gltf.Asset.Generator);
                Console.WriteLine("glTF version: " + gltf.Asset.Version);
                var glbfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".glb" : o.Output);
                var batchfile = (o.Output == string.Empty ? Path.GetFileNameWithoutExtension(o.Input) + ".batch" : o.Output);

                if (File.Exists(glbfile) && !o.Force)
                {
                    Console.WriteLine($"File {glbfile} already exists. Specify -f or --force to overwrite existing files.");
                }
                else
                {
                    gltf.SaveGLB(glbfile);
                    File.WriteAllBytes(glbfile, i3dm.GlbData);
                    Console.WriteLine($"Glb created: {glbfile}");
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
            catch (InvalidDataException ex)
            {
                Console.WriteLine("glTF version not supported.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
