using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetModelDefinition
    {
        [PropertyOrder(0)]
        [BigEndian]
        public short ModelNameLength { get; set; }

        [PropertyOrder(1)]
        [RelativeArraySize("ModelNameLength")]
        public string ModelName { get; set; }

        [PropertyOrder(2)]
        [BigEndian]
        public short ModelOffsetCount { get; set; }

        [PropertyOrder(3)]
        [RelativeArraySize("ModelCount")]
        public MSetModelOffset[] ModelOffsets { get; set; }

        [PropertyOrder(4)]
        [ArraySize(3)]
        public int[] Padding { get; set; }
    }
}
