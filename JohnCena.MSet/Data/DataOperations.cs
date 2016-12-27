using System;

namespace JohnCena.Mset.Data
{
    internal static class DataOperations
    {
        public static int GetDataTypeSize(DataType type)
        {
            switch (type)
            {
                case DataType.I8S:
                case DataType.I8U:
                case DataType.Char:
                    return 1;

                case DataType.I16S:
                case DataType.I16U:
                    return 2;

                case DataType.I32S:
                case DataType.I32U:
                case DataType.F32:
                    return 4;

                case DataType.I64S:
                case DataType.I64U:
                case DataType.F64:
                    return 8;

                default:
                    return -1;
            }
        }

        public static DataType GetDataType(Type type)
        {
            var t = type;
            if (t.IsArray)
                t = t.GetElementType();

            if (t == typeof(byte))
                return DataType.I8U;
            else if (t == typeof(sbyte))
                return DataType.I8S;
            else if (t == typeof(ushort))
                return DataType.I16U;
            else if (t == typeof(short))
                return DataType.I16S;
            else if (t == typeof(uint))
                return DataType.I32U;
            else if (t == typeof(int))
                return DataType.I32S;
            else if (t == typeof(ulong))
                return DataType.I64U;
            else if (t == typeof(long))
                return DataType.I64S;
            else if (t == typeof(float))
                return DataType.F32;
            else if (t == typeof(double))
                return DataType.F64;
            else if (t == typeof(string))
                return DataType.Char;

            return DataType.Unknown;
        }

        public static DataType GetDataType(object o)
        {
            return GetDataType(o.GetType());
        }
        
        public static object GetData(byte[] buff, int index, DataType type)
        {
            switch (type)
            {
                case DataType.Char:
                    return (char)buff[index];

                case DataType.F32:
                    return BitConverter.ToSingle(buff, index);

                case DataType.F64:
                    return BitConverter.ToDouble(buff, index);

                case DataType.I8S:
                    return (sbyte)buff[index];

                case DataType.I8U:
                    return buff[index];

                case DataType.I16S:
                    return BitConverter.ToInt16(buff, index);

                case DataType.I16U:
                    return BitConverter.ToUInt16(buff, index);

                case DataType.I32S:
                    return BitConverter.ToInt32(buff, index);

                case DataType.I32U:
                    return BitConverter.ToUInt32(buff, index);

                case DataType.I64S:
                    return BitConverter.ToInt64(buff, index);

                case DataType.I64U:
                    return BitConverter.ToUInt64(buff, index);

                default:
                    return null;
            }
        }

        public static T[] Cast<T>(object[] array)
        {
            var arr = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
                arr[i] = (T)array[i];
            return arr;
        }

        public static void SwapEndianness(byte[] array)
        {
            SwapEndianness(array, 0, array.Length);
        }

        public static void SwapEndianness(byte[] array, int index, int length)
        {
            Array.Reverse(array, index, length);
        }

        public static void SwapEndianness(byte[] array, int index, DataType data_type)
        {
            SwapEndianness(array, index, GetDataTypeSize(data_type));
        }

        public static void SwapEndianness(byte[] array, int index, DataType[] data_types)
        {
            int[] lengths = new int[data_types.Length];
            for (int i = 0; i < lengths.Length; i++)
                lengths[i] = GetDataTypeSize(data_types[i]);
            SwapEndianness(array, index, lengths);
        }

        public static void SwapEndianness(byte[] array, int index, int[] lengths)
        {
            int i = index;
            foreach(var len in lengths)
            {
                SwapEndianness(array, i, len);
                i += len;
            }
        }

        public static byte[] DeltaDecompress(byte[] source, DataType data_type, int stride, int count)
        {
            int dsize = GetDataTypeSize(data_type);
            if (dsize == 8)
                throw new InvalidOperationException("64-bit data is not supported");

            int tsize = count * stride * dsize;
            byte[] target = new byte[tsize];
            byte[] buff = new byte[dsize];

            var ctype = 0;
            var cbit = 0;

            var deltaI = 0;
            var deltaF = 0F;
            var fscale = 0F;
            var fscaleinv = 0F;

            var x = (2 * count * stride + 7) / 8;
            var y = 0;

            var prvI = new int[4];
            var prvF = new float[4];

            fscale = (float)(1 << source[x++]);
            if (fscale != 0)
                fscaleinv = 1.0F / fscale;

            for (int i = 0; i < count; i++)
                for (int j = 0; j < stride; j++)
                {
                    ctype = (source[cbit >> 3] >> (cbit & 7)) & 3;
                    cbit += 2;

                    switch (ctype)
                    {
                        case 0:
                        default:
                            deltaI = 0;
                            break;

                        case 1:
                            deltaI = source[x] - 0x7F;
                            x++;
                            break;

                        case 2:
                            deltaI = (source[x] | (source[x + 1] << 8)) - 0x7FFF;
                            x += 2;
                            break;

                        case 3:
                            deltaI = source[x] | (source[x + 1] << 8) | (source[x + 2] << 16) | (source[x + 3] << 24);
                            x += 4;
                            break;
                    }

                    switch (data_type)
                    {
                        case DataType.F32:
                            if (ctype != 3)
                                deltaF = deltaI * fscaleinv;
                            else
                                deltaF = BitConverter.ToSingle(BitConverter.GetBytes(deltaI), 0);

                            prvF[j] = prvF[j] + deltaF;
                            buff = BitConverter.GetBytes(prvF[j]);
                            break;

                        case DataType.I8U:
                        case DataType.I8S:
                        case DataType.I16U:
                        case DataType.I16S:
                        case DataType.I32U:
                        case DataType.I32S:
                            prvI[j] = prvI[j] + deltaI + 1;
                            buff = BitConverter.GetBytes(prvI[j]);
                            break;

                        default:
                            buff = new byte[4];
                            break;
                    }

                    Array.Copy(buff, 4 - dsize, target, y * dsize, dsize);
                    y++;
                }

            return target;
        }
    }
}
