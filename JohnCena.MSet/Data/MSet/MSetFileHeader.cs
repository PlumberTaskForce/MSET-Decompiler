using JohnCena.Mset.Data.Attributes;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetFileHeader
    {
        [PropertyOrder(0)]
        public int HeaderSize { get; set; }

        [PropertyOrder(1)]
        [BigEndian]
        public int FileVersion { get; set; }

        [PropertyOrder(2)]
        [BigEndian]
        public int DataCRC { get; set; }

        [PropertyOrder(3)]
        [BigEndian]
        public short ModelDefinitionCount { get; set; }

        [PropertyOrder(4)]
        [RelativeArraySize("ModelDefinitionCount")]
        public MSetModelDefinition[] ModelDefinitions { get; set; }
    }
}
