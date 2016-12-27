using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetModelDataOffset
    {
        [PropertyOrder(0)]
        [BigEndian]
        public int CompressedSize { get; set; }

        [PropertyOrder(1)]
        [BigEndian]
        public int DecompressedSize { get; set; }

        [PropertyOrder(2)]
        [BigEndian]
        public int Offset { get; set; }

        [Ignore]
        public bool IsEncoded { get; set; }
    }
}
