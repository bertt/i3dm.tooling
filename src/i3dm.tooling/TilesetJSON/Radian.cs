using System;

namespace i3dm.tooling.TilesetJSON
{
    public class Radian
    {
        public static double ToRadius(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
    }
}