using System;
using System.Numerics;


namespace i3dm.tooling.TilesetJSON
{
    public class LocalSystem
    {
        public static double[] GetLocalTransform(decimal[] scale, double heading, Vector3 relativeCenter)
        {
            double[] transform;
            var res = GetLocalEnuMapbox(heading);
            var m = Matrix.GetMatrix(relativeCenter, res.East, res.North, res.Up);
            transform = Flatten.Flattener(m, scale);
            return transform;
        }

        public static (Vector3 East, Vector3 North, Vector3 Up) GetLocalEnuMapbox(double angle)
        {
            var decimals = 6;
            var radian = Radian.ToRadius(angle);
            var east = new Vector3((float)Math.Round(Math.Cos(radian), decimals), (float)Math.Round(Math.Sin(radian) * -1, decimals), 0);
            var up = new Vector3(0, 0, 1);
            var north = new Vector3((float)Math.Round(Math.Sin(radian), decimals), (float)Math.Round(Math.Cos(radian), decimals), 0);
            return (east, north, up);
        }

    }
}