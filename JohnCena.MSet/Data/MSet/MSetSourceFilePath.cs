using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetSourceFilePath
    {
        [PropertyOrder(0)]
        public short PathLength { get; set; }

        [PropertyOrder(1)]
        [RelativeArraySize("PathLength")]
        public string Path { get; set; }

        [PropertyOrder(2)]
        [CalculateSize("PaddingSize", "PathLength")]
        public byte[] Padding { get; set; }

        [PropertyOrder(3)]
        public int Timestamp { get; set; }

        internal static int PaddingSize(int len)
        {
            return (4 - ((len + 6) & 3)) & 3;
        }
    }
}
