namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDTangent
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public ThreeDTangent(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
