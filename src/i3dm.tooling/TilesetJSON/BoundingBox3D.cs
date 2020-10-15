using System.Numerics;
using System.Globalization;
using System.Collections.Generic;

namespace i3dm.tooling.TilesetJSON
{
    public class BoundingBox3D
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }

        public BoundingBox3D() { }

        public BoundingBox3D(double XMin, double YMin, double ZMin, double XMax, double YMax, double ZMax)
        {
            this.XMin = XMin;
            this.YMin = YMin;
            this.ZMin = ZMin;
            this.XMax = XMax;
            this.YMax = YMax;
            this.ZMax = ZMax;
        }

        public BoundingBox3D(List<Vector3> positions)
        {
            XMin = XMax = YMin = YMax = ZMin = ZMax = double.MinValue;

            for(var i = 0; i < positions.Count; i++)
            {
                var p = positions[i];
                XMin = CompareBBoxValue(XMin, p.X, true);
                YMin = CompareBBoxValue(YMin, p.Y, true);
                ZMin = CompareBBoxValue(ZMin, p.Z, true);
                XMax = CompareBBoxValue(XMax, p.X, false);
                YMax = CompareBBoxValue(YMax, p.Y, false);
                ZMax = CompareBBoxValue(ZMax, p.Z, false);
            }
        }

        public Vector3 GetCenter()
        {
            var x = (XMax + XMin) / 2;
            var y = (YMax + YMin) / 2;
            var z = (ZMax + ZMin) / 2;
            return new Vector3((float)x, (float)y, (float)z);
        }

        public override string ToString()
        {
            return $"{XMin.ToString(CultureInfo.InvariantCulture)},{YMin.ToString(CultureInfo.InvariantCulture)},{ZMin.ToString(CultureInfo.InvariantCulture)},{XMax.ToString((CultureInfo.InvariantCulture))},{YMax.ToString((CultureInfo.InvariantCulture))},{ZMax.ToString((CultureInfo.InvariantCulture))}";
        }

        public double ExtentX()
        {
            //var max = SpatialConvertor.ToSphericalMercatorFromWgs84((double)XMax, (double)YMax);
            //var min = SpatialConvertor.ToSphericalMercatorFromWgs84((double)XMin, (double)YMin);
            return (XMax - XMin);
        }
        public double ExtentY()
        {
            //var max = SpatialConvertor.ToSphericalMercatorFromWgs84((double)XMax, (double)YMax);
            //var min = SpatialConvertor.ToSphericalMercatorFromWgs84((double)XMin, (double)YMin);
            return (YMax -YMin);
        }
        public double ExtentZ()
        {
            return (ZMax - ZMin);
        }

        private double CompareBBoxValue(double curVal, double newVal, bool lower)
        {
            if (curVal == double.MinValue || (lower ? newVal < curVal : newVal > curVal))
            {
                return newVal;
            }

            return curVal;
        }
    }
}