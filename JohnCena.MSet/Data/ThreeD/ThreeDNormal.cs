namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDNormal
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public ThreeDNormal(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
