using System;

namespace i3dm.tooling;
public static class SpatialConverter
{
    const double a = 6378137.0;         // WGS-84 Earth semimajor axis (m)

    const double b = 6356752.314245;     // Derived Earth semiminor axis (m)
    const double f = (a - b) / a;           // Ellipsoid Flatness
    const double f_inv = 1.0 / f;       // Inverse flattening

    //const double f_inv = 298.257223563; // WGS-84 Flattening Factor of the Earth 
    //const double b = a - a / f_inv;
    //const double f = 1.0 / f_inv;

    const double a_sq = a * a;
    const double b_sq = b * b;
    const double e_sq = f * (2 - f);    // Square of Eccentricity

    public static void EcefToGeodetic(double x, double y, double z,
                                    out double lat, out double lon, out double h)
    {
        var eps = e_sq / (1.0 - e_sq);
        var p = Math.Sqrt(x * x + y * y);
        var q = Math.Atan2((z * a), (p * b));
        var sin_q = Math.Sin(q);
        var cos_q = Math.Cos(q);
        var sin_q_3 = sin_q * sin_q * sin_q;
        var cos_q_3 = cos_q * cos_q * cos_q;
        var phi = Math.Atan2((z + eps * b * sin_q_3), (p - e_sq * a * cos_q_3));
        var lambda = Math.Atan2(y, x);
        var v = a / Math.Sqrt(1.0 - e_sq * Math.Sin(phi) * Math.Sin(phi));
        h = (p / Math.Cos(phi)) - v;

        lat = RadiansToDegrees(phi);
        lon = RadiansToDegrees(lambda);
    }

    static double RadiansToDegrees(double radians)
    {
        return 180.0 / Math.PI * radians;
    }
}
