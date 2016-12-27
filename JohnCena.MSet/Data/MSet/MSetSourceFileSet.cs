using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetSourceFileSet
    {
        [PropertyOrder(0)]
        public short FileSetNameLength { get; set; }

        [PropertyOrder(1)]
        [RelativeArraySize("FileSetNameLength")]
        public string FileSetName { get; set; }

        [PropertyOrder(2)]
        public int SetLength { get; set; }

        [PropertyOrder(3)]
        public int FileCount { get; set; }

        [PropertyOrder(4)]
        [RelativeArraySize("FileCount")]
        public MSetSourceFilePath[] Files { get; set; }
    }
}
