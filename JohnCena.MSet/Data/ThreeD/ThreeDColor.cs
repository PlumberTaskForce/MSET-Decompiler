namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public float RFloat { get { return (float)this.R / 255F; } }
        public float GFloat { get { return (float)this.G / 255F; } }
        public float BFloat { get { return (float)this.B / 255F; } }
        public float AFloat { get { return (float)this.A / 255F; } }

        public ThreeDColor(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
    }
}
