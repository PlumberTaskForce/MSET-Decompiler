using System.IO;
using System.Text;

namespace JohnCena.Mset.Data
{
    internal static class BinaryReaderExtensions
    {
        internal static object ReadDataType(this BinaryReader br, DataType data_type)
        {
            var raw = br.ReadBytes(DataOperations.GetDataTypeSize(data_type));
            return DataOperations.GetData(raw, 0, data_type);
        }

        internal static object ReadDataTypeBig(this BinaryReader br, DataType data_type)
        {
            var raw = br.ReadBytes(DataOperations.GetDataTypeSize(data_type));
            DataOperations.SwapEndianness(raw);
            return DataOperations.GetData(raw, 0, data_type);
        }

        internal static byte[] ReadDataTypesRaw(this BinaryReader br, params DataType[] data_types)
        {
            var length = 0;
            for (int i = 0; i < data_types.Length; i++)
                length += DataOperations.GetDataTypeSize(data_types[i]);

            return br.ReadBytes(length);
        }

        internal static object[] ReadDataTypes(this BinaryReader br, params DataType[] data_types)
        {
            var raw = br.ReadDataTypesRaw(data_types);
            var data = RawToData(raw, data_types);
            return data;
        }

        internal static object[] ReadDataTypesBig(this BinaryReader br, params DataType[] data_types)
        {
            var raw = br.ReadDataTypesRaw(data_types);
            DataOperations.SwapEndianness(raw, 0, data_types);
            var data = RawToData(raw, data_types);
            return data;
        }

        internal static string ReadString(this BinaryReader br, int length, Encoding encoding)
        {
            var raw = br.ReadBytes(length);
            return encoding.GetString(raw);
        }

        private static object[] RawToData(byte[] raw, params DataType[] data_types)
        {
            var data = new object[data_types.Length];

            var i = 0;
            for (int j = 0; j < data_types.Length; j++)
            {
                data[j] = DataOperations.GetData(raw, i, data_types[j]);
                i += DataOperations.GetDataTypeSize(data_types[j]);
            }

            return data;
        }
    }
}
