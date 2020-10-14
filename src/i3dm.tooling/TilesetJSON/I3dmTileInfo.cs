using System;
using System.Collections.Generic;
using System.Numerics;

namespace i3dm.tooling.TilesetJSON
{
    public class I3dmTileInfo
    {
        public string Name;
        public int Row;
        public int Col;

        public double MinX;
        public double MaxX;
        public double MinY;
        public double MaxY;

        public byte[] File = null;
        public string BatchTableJson = null;
        public string FeatureTableJson = null;
        public List<Vector3> Positions = new List<Vector3>();
        public List<float> Scales = null;
        public List<Vector3> NormalsUp = null;
        public List<Vector3> NormalsRight = null;
        public List<Vector3> ScaleNonUniforms = null;

        public I3dmTileInfo() { }

        public I3dmTileInfo(string name, int row, int col, double minX, double maxX, double minY, double maxY)
        {
            Name = name;
            Row = row;
            Col = col;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public Vector3 GetCenter()
        {
            var x = (MaxX + MinX) / 2;
            var y = (MaxY + MinY) / 2;
            var z = (0 + 50) / 2;
            return new Vector3((float)x, (float)y, (float)z);
        }

        public string TileName {
            get { return $"{Name}_{Row}_{Col}.i3dm"; }
        }

        public double[] GetTransform(Vector3 centroid)
        {
            var distance = DistanceCalculator.Distance(centroid, GetCenter());
            return LocalSystem.GetLocalTransform(new decimal[]{1M, 1M, 1M}, 0, distance);
        }

        public BoundingBox3D GetBounds(){
            return new BoundingBox3D(MinX, MinY, 0, MaxX, MaxY, 100); 
        }
    }
}