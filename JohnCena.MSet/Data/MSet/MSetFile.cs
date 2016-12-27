using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetFile
    {
        [PropertyOrder(0)]
        public MSetFileHeader Header { get; set; }

        [PropertyOrder(1)]
        public MSetSourceFileSet SourceFileSet { get; set; }

        [PropertyOrder(2)]
        public MSetModel[] Models { get; set; }

        [Ignore]
        public MSetZlibStream[] Data { get; set; }
    }
}
