using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetModelOffset
    {
        [PropertyOrder(0)]
        [BigEndian]
        public int Offset { get; set; }

        [PropertyOrder(1)]
        [BigEndian]
        public int Length { get; set; }
    }
}
