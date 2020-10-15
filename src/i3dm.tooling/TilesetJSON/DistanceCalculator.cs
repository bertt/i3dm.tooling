using System.Numerics;

namespace i3dm.tooling.TilesetJSON
{
    public static class DistanceCalculator
    {
        public static Vector3 Distance(Vector3 from, Vector3 to)
        {
            //var fromSpherical = SpatialConvertor.ToSphericalMercatorFromWgs84((double)from.X, (double)from.Y);
            //var toSpherical = SpatialConvertor.ToSphericalMercatorFromWgs84((double)to.X, (double)to.Y);
            return new Vector3( (float)to.X - (float)from.X,  (float)to.Y - (float)from.Y, 0);
        }
    }
}