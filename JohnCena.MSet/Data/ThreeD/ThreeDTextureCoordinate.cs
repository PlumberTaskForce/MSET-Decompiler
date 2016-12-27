namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDTextureCoordinate
    {
        public float U { get; set; }
        public float V { get; set; }

        public ThreeDTextureCoordinate(float u, float v)
            : this()
        {
            this.U = u;
            this.V = v;
        }
    }
}
