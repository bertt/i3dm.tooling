using System.Numerics;

namespace i3dm.tooling.TilesetJSON
{
    public class MapBoxTransformer
    {
        public static double[] GetTransform(Vector3 p, decimal[] scale, double heading)
        {
            //var position = SpatialConvertor.ToSphericalMercatorFromWgs84((double)p.X, (double)p.Y);
            var center = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
            var transform = LocalSystem.GetLocalTransform(scale, heading, center);
            return transform;
        }
    }
}