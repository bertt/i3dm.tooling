using Newtonsoft.Json;
using System.Collections.Generic;
using i3dm.tooling.Utils;

namespace i3dm.tooling.TilesetJSON
{
    public class Tileset
    {
        public List<I3dmTileInfo> I3DMTiles { get; set; }
        public BoundingBox3D BoundingBox3D { get; set; }

        public Tileset(List<I3dmTileInfo> tiles, BoundingBox3D bb3d)
        {
            I3DMTiles = tiles;
            BoundingBox3D = bb3d;
        }

        public double[] GetRootTransform()
        {
            var centroid = BoundingBox3D.GetCenter();
            double[] transformRoot = MapBoxTransformer.GetTransform(centroid, new decimal[] { 1, 1, 1 }, 0);
            return transformRoot;
        }

        public string GetTileSetJson()
        {
            var tileset = GetTileSet();
            var json = JsonConvert.SerializeObject(tileset, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return json;
        }

        public TileSetJson GetTileSet()
        {
            var geometricError = 500;
            var extent_x = BoundingBox3D.ExtentX();
            var extent_y = BoundingBox3D.ExtentY();
            var extent_z = 100;

            var tileset = new TileSetJson
            {
                asset = new Asset() { version = "1.0", generator = "i3dm.tooling" }
            };

            var box = new double[] { 0, 0, 0, extent_x / 2, 0.0, 0.0, 0.0, extent_y / 2, 0.0, 0.0, 0.0, extent_z };

            var boundingVolume = new Boundingvolume
            {
                box = box
            };

            var root = new Root
            {
                geometricError = geometricError,
                refine = "REPLACE",
                transform = MathUtils.Round(GetRootTransform(), 8),
                boundingVolume = boundingVolume
            };

            var centroid = BoundingBox3D.GetCenter();
            var children = new List<Child>();
            foreach (var tile in I3DMTiles)
            {
                var child = new Child();
                child.geometricError = 0;
                child.content = new Content() { uri = tile.TileName };
                var tileTransform = tile.GetTransform(centroid);
                child.transform = MathUtils.Round(tileTransform, 8);
                var tileBounds = tile.GetBounds();
                var bbChild = new Boundingvolume();
                bbChild.box = new double[] { 0, 0, 0, tileBounds.ExtentX() / 2, 0.0, 0.0, 0.0, tileBounds.ExtentY() / 2, 0.0, 0.0, 0.0, tileBounds.ExtentZ() / 2};
                child.boundingVolume = bbChild;
                children.Add(child);
            }

            root.children = children;
            tileset.root = root;
            tileset.geometricError = 500;
            return tileset;
        }
    }
}