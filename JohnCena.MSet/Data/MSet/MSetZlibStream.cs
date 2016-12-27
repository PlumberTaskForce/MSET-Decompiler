using System.IO;

namespace JohnCena.Mset.Data.MSet
{
    internal struct MSetZlibStream
    {
        public Stream Data { get; set; }
        public byte[] DataProcessed { get; set; }

        public int DecompressedLength { get; set; }
        public int CompressedLength { get; set; }
        public bool IsEncoded { get; set; }

        public int Model { get; set; }
        public int DataStream { get; set; }
        public string ModelName { get; set; }
    }
}
