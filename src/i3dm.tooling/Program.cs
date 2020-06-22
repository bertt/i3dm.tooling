using CommandLine;
using I3dm.Tile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace i3dm.tooling
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parser.Default.ParseArguments<PackOptions, UnpackOptions, InfoOptions>(args).WithParsed(o =>
            Parser.Default.ParseArguments<InfoOptions>(args).WithParsed(o =>
            {
                switch (o)
                {
                    case InfoOptions options:
                        Info(options);
                        break;
                }
            });

        }

        static void Info(InfoOptions o)
        {
            Console.WriteLine($"Action: Info");
            Console.WriteLine("i3dm file: " + o.Input);
            var f = File.OpenRead(o.Input);
            var i3dm = I3dmReader.ReadI3dm(f);
            Console.WriteLine("i3dm header version: " + i3dm.I3dmHeader.Version);
            Console.WriteLine("i3dm GltfFormat: " + i3dm.I3dmHeader.GltfFormat);
            Console.WriteLine("i3dm header magic: " + i3dm.I3dmHeader.Magic);
            Console.WriteLine("i3dm header bytelength: " + i3dm.I3dmHeader.ByteLength);
            Console.WriteLine("i3dm header featuretablejson length: " + i3dm.I3dmHeader.FeatureTableJsonByteLength);
            Console.WriteLine("i3dm header batchtablejson length: " + i3dm.I3dmHeader.BatchTableJsonByteLength);
            Console.WriteLine("Batch table json: " + i3dm.BatchTableJson);
            Console.WriteLine("Feature table json: " + i3dm.FeatureTableJson);
            PrintVector3(i3dm.FeatureTable.Positions, "positions: ");
            PrintVector3(i3dm.FeatureTable.NormalUps, "normal ups: ");
            PrintVector3(i3dm.FeatureTable.NormalRights, "normal rights: ");
            PrintVector3(i3dm.FeatureTable.ScaleNonUniforms, "Scale non-uniform: ");

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
                    // Console.WriteLine("glTF primitive attributes: " + String.Join(',', glb.LogicalMeshes[0].Primitives[0].Attributes));
                    // gltf.Meshes[0].Primitives[0];
                    // todo: how to get to the vertices?
                }
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine("glTF version not supported.");
                Console.WriteLine(ex.Message);
            }
            f.Dispose();
        }

        private static void PrintVector3(List<Vector3> vectors, string name)
        {
            if (vectors != null)
            {
                Console.WriteLine(name + string.Join(',', vectors));
            }
        }
    }
}
