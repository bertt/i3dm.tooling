using System.Numerics;
namespace i3dm.tooling.TilesetJSON
{
    public class Tile
    {
        public string Id { get; set; }

        public Tile()
        {

        }

        public Tile(string Id, byte[] Glb, Vector3 Position, double Heading, decimal[] Scale)
        {
            this.Id = Id;
            this.Glb = Glb;
            this.Position = Position;
            this.Heading = Heading;
            this.Scale = Scale;
        }

        public byte[] Glb { get; set; }
        public Vector3 Position { get; set; }
        public double Heading { get; set; }
        public decimal[] Scale { get; set; }

        public double[] GetTransform(Vector3 centroid)
        {
            var distance = DistanceCalculator.Distance(centroid, Position);
            // todo: fix scale
            return LocalSystem.GetLocalTransform(Scale, Heading, distance);
        }
    }

}