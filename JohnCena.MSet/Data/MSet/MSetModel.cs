using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetModel
    {
        [PropertyOrder(0)]
        [BigEndian]
        public int DataSize { get; set; } // The size of this structure plus all persistent packed data

        [PropertyOrder(1)]
        [BigEndian]
        public int VertexCount { get; set; }

        [PropertyOrder(2)]
        [BigEndian]
        public int FaceCount { get; set; }

        [PropertyOrder(3)]
        [BigEndian]
        public int TextureCount { get; set; } //number of tex_idxs (sum of all tex_idx->counts == tri_count)

        [PropertyOrder(4)]
        [BigEndian]
        public float AverageTexelDensity { get; set; }

        [PropertyOrder(5)]
        [BigEndian]
        public float StdDevTexelDensity { get; set; }

        [PropertyOrder(6)]
        [BigEndian]
        public int ProcessTimeFlags { get; set; }

        [PropertyOrder(7)]
        [BigEndian]
        public int Unknown1C { get; set; }

        [PropertyOrder(8)]
        [ArraySize(12)]
        public MSetModelDataOffset[] ModelDataOffsets { get; set; }

        //metadata
        [Ignore]
        public int MasterOffset { get; set; }

        [Ignore]
        public string ModelName { get; set; }
    }
}
