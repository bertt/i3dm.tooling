using System.IO;
using System;
using i3dm.tooling.Utils;
using I3dm.Tile;
using CommandLine;
using i3dm.tooling.TilesetJSON;
using System.Collections.Generic;
using System.Numerics;

namespace i3dm.tooling.Commands
{
    [Verb("pack", HelpText = "pack glb to i3dm")]
    public class Pack : ICommand
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the glb file")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, Default = "", HelpText = "Output path of the resulting .i3dm")]
        public string Output { get; set; }

        [Option('s', "size", Required = false, Default = double.MinValue, HelpText = "Double to set the tilesize based on position units")]
        public double Tilesize { get; set; }

        [Option('f', "force", Required = false, Default = false, HelpText = "force overwrite output file")]
        public bool Force { get; set; }

        public void Run()
        {
            Console.WriteLine($"Action: Pack");
            Console.WriteLine($"Input: {Input}");
            var f = File.ReadAllBytes(Input);
            var dir = Path.GetDirectoryName(Input);
            var fileName = Path.GetFileNameWithoutExtension(Input);
            var batchTableJsonFile = Path.Combine(dir, fileName + ".batch.csv");
            var featureTableJsonFile = Path.Combine(dir, fileName + ".feature.csv");
            var positionsFile = Path.Combine(dir, fileName + ".positions.csv");
            var normal_upsfile = Path.Combine(dir, fileName + ".normal_ups.csv");
            var normal_rightsfile = Path.Combine(dir, fileName + ".normal_rights.csv");
            var scale_non_uniformsfile = Path.Combine(dir, fileName + ".scale_non_uniforms.csv");
            var scalesfile = Path.Combine(dir, fileName + ".scales.csv");
            var positions = FileUtils.ReadVectors(positionsFile);

            string batchTableJson = null;
            string featureTableJson = null;
            List<Vector3> normal_ups = null;
            List<Vector3> normal_rights = null;
            List<Vector3> scale_non_uniforms = null;
            List<float> scales = null;

            if (File.Exists(batchTableJsonFile))
            {
                Console.WriteLine($"Input batchtable json file: {batchTableJsonFile}");
                batchTableJson = File.ReadAllText(batchTableJsonFile);
            }
            if (File.Exists(featureTableJsonFile))
            {
                Console.WriteLine($"Input featureTable json file: {featureTableJsonFile}");
                featureTableJson = File.ReadAllText(featureTableJsonFile);
            }
            if (File.Exists(normal_upsfile))
            {
                Console.WriteLine($"Input normal_upsfile file: {normal_upsfile}");
                normal_ups = FileUtils.ReadVectors(normal_upsfile);
            }
            if (File.Exists(normal_rightsfile))
            {
                Console.WriteLine($"Input normal_rightsfile file: {normal_rightsfile}");
                normal_rights = FileUtils.ReadVectors(normal_rightsfile);
            }
            if (File.Exists(scale_non_uniformsfile))
            {
                Console.WriteLine($"Input scale_non_uniforms file: {scale_non_uniformsfile}");
                scale_non_uniforms = FileUtils.ReadVectors(scale_non_uniformsfile);
            }
            if (File.Exists(scalesfile))
            {
                Console.WriteLine($"Input scales file: {scalesfile}");
                scales = FileUtils.ReadFloats(scalesfile);
            }

            var bounds = new BoundingBox3D(positions);
            var name = string.IsNullOrEmpty(Output) ? Path.GetFileNameWithoutExtension(Input) : Path.GetFileNameWithoutExtension(Output);
            var tileInfos = GenerateTileInfos(bounds, name, f, positions, batchTableJson, featureTableJson, scales, normal_ups, normal_rights, scale_non_uniforms);

            exportTiles(tileInfos);
            exportTilesetJSON(bounds, tileInfos);
        }

        private void exportTiles(List<I3dmTileInfo> tiles)
        {
            foreach (var tile in tiles)
            {
                var i3dm = new I3dm.Tile.I3dm(tile.Positions, tile.File);

                if (!string.IsNullOrEmpty(tile.BatchTableJson))
                {
                    i3dm.BatchTableJson = tile.BatchTableJson;
                }

                if (!string.IsNullOrEmpty(tile.FeatureTableJson))
                {
                    i3dm.FeatureTableJson = tile.FeatureTableJson;
                }
                if (tile.NormalsUp != null)
                {
                    i3dm.NormalUps = tile.NormalsUp;
                }
                if (tile.NormalsRight != null)
                {
                    i3dm.NormalRights = tile.NormalsRight;
                }
                if (tile.ScaleNonUniforms != null)
                {
                    i3dm.ScaleNonUniforms = tile.ScaleNonUniforms;
                }
                if (tile.Scales != null)
                {
                    i3dm.Scales = tile.Scales;
                }

                var file = (Output == string.Empty ? tile.TileName : Path.Combine(Path.GetDirectoryName(Output), tile.TileName));

                if (File.Exists(file) && !Force)
                {
                    Console.WriteLine($"File {file} already exists. Specify -f or --force to overwrite existing files.");
                }
                else
                {
                    I3dmWriter.Write(file, i3dm);
                    Console.WriteLine("I3dm created " + file);
                }
            }
        }

        private void exportTilesetJSON(BoundingBox3D bounds, List<I3dmTileInfo> tiles)
        {
            var tileset = new Tileset(tiles, bounds);
            var json = tileset.GetTileSetJson();
            var jsonFile = (Output == string.Empty ? Path.GetFileNameWithoutExtension(Input) + "tileset.json" : Path.Combine(Path.GetDirectoryName(Output), Path.GetFileNameWithoutExtension(Output) + "_tileset.json"));

            using (StreamWriter outputFile = new StreamWriter(jsonFile))
            {
                outputFile.WriteLine(json);
            }
        }

        private List<I3dmTileInfo> GenerateTileInfos(BoundingBox3D bounds, string name, byte[] file, List<Vector3> positions, string batchTableJson, string featureTableJson, List<float> scales, List<Vector3> normalsUp, List<Vector3> normalsRight, List<Vector3> scaleNonUniforms)
        {
            if (Tilesize != double.MinValue)
            {
                var tiles = new List<I3dmTileInfo>();
                var rowCount = Math.Ceiling((bounds.XMax - bounds.XMin) / Tilesize);
                var columnCount = Math.Ceiling((bounds.YMax - bounds.YMin) / Tilesize);
                Console.WriteLine($"Tilesize set to {Tilesize}, based on input data this will result in {(rowCount * columnCount)} tiles");

                for (var i = 0; i < rowCount; i++)
                {
                    for (var j = 0; j < columnCount; j++)
                    {
                        var minX = bounds.XMin + (i * Tilesize);
                        var maxX = minX + Tilesize;
                        var maxY = bounds.YMax - (j * Tilesize);
                        var minY = maxY - Tilesize;
                        var tile = new I3dmTileInfo(name, i, j, minX, maxX, minY, maxY);
                        tile.File = file;
                        tiles.Add(tile);
                    }
                }

                SetTileInfos(tiles, positions, batchTableJson, featureTableJson, scales, normalsUp, normalsRight, scaleNonUniforms);
                return tiles;
            }

            return new List<I3dmTileInfo> { new I3dmTileInfo(name, 0, 0, bounds.XMin, bounds.XMax, bounds.YMin, bounds.YMax){
                File = file,
                Positions = positions,
                BatchTableJson = batchTableJson,
                FeatureTableJson = featureTableJson,
                Scales = scales,
                NormalsUp = normalsUp,
                NormalsRight = normalsRight,
                ScaleNonUniforms = scaleNonUniforms
            } };
        }

        private void SetTileInfos(List<I3dmTileInfo> tileInfos, List<Vector3> positions, string BatchTableJson, string featureTableJson, List<float> scales, List<Vector3> normalsUp, List<Vector3> normalsRight, List<Vector3> ScaleNonUniforms)
        {
            if (Tilesize != Int16.MinValue)
            {
                for (var i = 0; i < positions.Count; i++)
                {
                    for (var j = 0; j < tileInfos.Count; j++)
                    {
                        var intersects = Intersects(positions[i], tileInfos[j].MinX, tileInfos[j].MaxX, tileInfos[j].MinY, tileInfos[j].MaxY);
                        if (!intersects)
                        {
                            continue;
                        }

                        var tile = tileInfos[j];
                        tile.Positions.Add(positions[i]);
                        tile.Scales = scales;
                    }
                }
            }
        }

        private bool Intersects(Vector3 position, double minX, double maxX, double minY, double maxY)
        {
            if (position.X >= minX && position.X <= maxX & position.Y >= minY && position.Y <= maxY)
            {
                return true;
            }
            return false;
        }
    }
}