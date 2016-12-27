namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDFace
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public int Vertex3 { get; set; }

        public ThreeDFace(int v1, int v2, int v3)
            : this()
        {
            this.Vertex1 = v1;
            this.Vertex2 = v2;
            this.Vertex3 = v3;
        }
    }
}
