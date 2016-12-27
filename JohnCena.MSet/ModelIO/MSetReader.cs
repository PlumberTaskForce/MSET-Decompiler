using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JohnCena.AdaptedLogger;
using JohnCena.Mset.Data;
using JohnCena.Mset.Data.MSet;
using JohnCena.Mset.Data.ThreeD;
using Ionic.Zlib;

namespace JohnCena.Mset.ModelIO
{
    internal sealed class MSetReader
    {
        private static char[] LegalAlphabet { get; set; }

        public MSetReader()
        {

        }

        static MSetReader()
        {
            LegalAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._-()".ToCharArray();
        }

        public MSetFile Load(string filename)
        {
            return this.Load(new FileInfo(filename));
        }

        public MSetFile Load(FileInfo file)
        {
            using (var fs = file.OpenRead())
                return this.Load(fs);
        }

        public MSetFile Load(Stream input_stream)
        {
            using (var br = new BinaryReader(input_stream))
                return this.Load(br);
        }

        public MSetFile Load(BinaryReader br)
        {
            return this.LoadInternal(br);
        }

        private MSetFile LoadInternal(BinaryReader br)
        {
            var ascii = new ASCIIEncoding();

            // HEADER
            var mh_hs = br.ReadInt32();
            var mh___ = br.ReadDataTypesBig(DataType.I32S, DataType.I32S, DataType.I16S);
            var mh_fv = (int)mh___[0];
            var mh_ts = (int)mh___[1];
            var mh_mc = (short)mh___[2];

            var mh_mdls = new MSetModelDefinition[mh_mc];
            for (int i = 0; i < mh_mc; i++)
            {
                var mh_mdl_nl = (short)br.ReadDataTypeBig(DataType.I16S);
                var mh_mdl_mn = br.ReadString(mh_mdl_nl, ascii);
                var mh_mdl_oc = (short)br.ReadDataTypeBig(DataType.I16S);

                var mh_mdl_ofs = new MSetModelOffset[mh_mdl_oc];
                for (int j = 0; j < mh_mdl_oc; j++)
                {
                    var mh_mdl_of___ = br.ReadDataTypesBig(DataType.I32S, DataType.I32S);
                    var mh_mdl_of_of = (int)mh_mdl_of___[0];
                    var mh_mdl_of_ln = (int)mh_mdl_of___[1];

                    var mh_mdl_of = new MSetModelOffset
                    {
                        Offset = mh_mdl_of_of,
                        Length = mh_mdl_of_ln
                    };
                    mh_mdl_ofs[j] = mh_mdl_of;
                }

                var mh_mdl_pd_r = br.ReadDataTypesBig(DataType.I32S, DataType.I32S, DataType.I32S);
                var mh_mdl_pd = DataOperations.Cast<int>(mh_mdl_pd_r);

                var mh_mdl = new MSetModelDefinition
                {
                    ModelNameLength = mh_mdl_nl,
                    ModelName = mh_mdl_mn,
                    ModelOffsetCount = mh_mdl_oc,
                    ModelOffsets = mh_mdl_ofs,
                    Padding = mh_mdl_pd
                };
                mh_mdls[i] = mh_mdl;
            }

            var mh = new MSetFileHeader
            {
                HeaderSize = mh_hs,
                FileVersion = mh_fv,
                DataCRC = mh_ts,
                ModelDefinitionCount = mh_mc,
                ModelDefinitions = mh_mdls
            };

            // SOURCE FILE SET
            var msf_nl = (short)br.ReadDataType(DataType.I16S);
            var msf_sn = br.ReadString(msf_nl, ascii);
            var msf___ = br.ReadDataTypes(DataType.I32S, DataType.I32S);
            var msf_sl = (int)msf___[0];
            var msf_fc = (int)msf___[1];

            var msf_fs = new MSetSourceFilePath[msf_fc];
            for (int i = 0; i < msf_fc; i++)
            {
                var msf_f_nl = (short)br.ReadDataType(DataType.I16S);
                var msf_f_fn = br.ReadString(msf_f_nl, ascii);
                var msf_f_pd = br.ReadBytes(MSetSourceFilePath.PaddingSize(msf_f_nl));
                var msf_f_ts = (int)br.ReadDataType(DataType.I32S);

                var msf_f = new MSetSourceFilePath
                {
                    PathLength = msf_f_nl,
                    Path = msf_f_fn,
                    Padding = msf_f_pd,
                    Timestamp = msf_f_ts
                };
                msf_fs[i] = msf_f;
            }

            var msf = new MSetSourceFileSet
            {
                FileSetNameLength = msf_nl,
                FileSetName = msf_sn,
                SetLength = msf_sl,
                FileCount = msf_fc,
                Files = msf_fs
            };

            // MODELS
            var x = 0;
            for (int i = 0; i < mh.ModelDefinitionCount; i++)
                for (int j = 0; j < mh.ModelDefinitions[i].ModelOffsetCount; j++)
                    if (mh.ModelDefinitions[i].ModelOffsets[j].Offset > 0)
                        x++;
            var mdls = new MSetModel[x];

            x = 0;
            for (int i = 0; i < mh.ModelDefinitionCount; i++)
            {
                var mdl_def = mh.ModelDefinitions[i];
                for (int j = 0; j < mdl_def.ModelOffsetCount; j++)
                {
                    var mdl_def_off = mdl_def.ModelOffsets[j];
                    if (mdl_def_off.Offset <= 0)
                        continue;

                    br.BaseStream.Seek(mdl_def_off.Offset, SeekOrigin.Begin);

                    var mdl___ = br.ReadDataTypesBig(DataType.I32S, DataType.I32S, DataType.I32S, DataType.I32S, DataType.F32, DataType.F32, DataType.I32S, DataType.I32S);
                    var mdl_so = (int)mdl___[0];
                    var mdl_vc = (int)mdl___[1];
                    var mdl_fc = (int)mdl___[2];
                    var mdl_0C = (int)mdl___[3];
                    var mdl_10 = (float)mdl___[4];
                    var mdl_14 = (float)mdl___[5];
                    var mdl_18 = (int)mdl___[6];
                    var mdl_1C = (int)mdl___[7];

                    var mdl_mdos = new MSetModelDataOffset[12];
                    for (int k = 0; k < 12; k++)
                    {
                        var mdl_mdo___ = br.ReadDataTypesBig(DataType.I32S, DataType.I32S, DataType.I32S);
                        var mdl_mdo_cs = (int)mdl_mdo___[0];
                        var mdl_mdo_us = (int)mdl_mdo___[1];
                        var mdl_mdo_of = (int)mdl_mdo___[2];

                        var mdl_mdo = new MSetModelDataOffset
                        {
                            CompressedSize = mdl_mdo_cs,
                            DecompressedSize = Math.Abs(mdl_mdo_us),
                            Offset = mdl_mdo_of,
                            IsEncoded = mdl_mdo_us > 0
                        };
                        mdl_mdos[k] = mdl_mdo;
                    }

                    var mdl = new MSetModel
                    {
                        DataSize = mdl_so,
                        VertexCount = mdl_vc,
                        FaceCount = mdl_fc,
                        TextureCount = mdl_0C,
                        AverageTexelDensity = mdl_10,
                        StdDevTexelDensity = mdl_14,
                        ProcessTimeFlags = mdl_18,
                        Unknown1C = mdl_1C,
                        ModelDataOffsets = mdl_mdos,
                        MasterOffset = mdl_def_off.Offset,
                        ModelName = mdl_def.ModelName
                    };
                    mdls[x++] = mdl;
                }
            }

            // ZLIB STREAMS
            x = 0;
            for (int i = 0; i < mdls.Length; i++)
            {
                var mdl = mdls[i];
                for (int j = 0; j < 12; j++)
                {
                    var mdl_mdo = mdl.ModelDataOffsets[j];
                    if (mdl_mdo.Offset <= 0)
                        continue;

                    x++;
                }
            }

            var mzs = new MSetZlibStream[x];
            var bs = br.BaseStream;

            x = 0;

            for (int i = 0; i < mdls.Length; i++)
            {
                var mdl = mdls[i];
                for (int j = 0; j < 12; j++)
                {
                    var mdl_mdo = mdl.ModelDataOffsets[j];
                    if (mdl_mdo.Offset <= 0)
                        continue;

                    var off = mdl_mdo.Offset + mdl.MasterOffset;
                    var len = mdl_mdo.CompressedSize;
                    if (len == 0 && mdl_mdo.DecompressedSize > 0)
                        len = mdl_mdo.DecompressedSize;
                    var len2 = len;

                    var bts = new byte[len];
                    bs.Seek(off, SeekOrigin.Begin);
                    while (len > 0)
                        len -= bs.Read(bts, len2 - len, len);

                    mzs[x++] = new MSetZlibStream
                    {
                        Data = new MemoryStream(bts),
                        DecompressedLength = mdl_mdo.DecompressedSize,
                        CompressedLength = mdl_mdo.CompressedSize,
                        IsEncoded = mdl_mdo.IsEncoded,
                        Model = i,
                        DataStream = j,
                        ModelName = mdl.ModelName
                    };
                }
            }

            // OUTPUT DATA
            var mset = new MSetFile
            {
                Header = mh,
                SourceFileSet = msf,
                Models = mdls,
                Data = mzs
            };
            return mset;
        }

        public IEnumerable<ThreeDModel> Create3DModels(MSetFile file)
        {
            var opts = Program.Options;
            var mzs = file.Data;
            var mns = new HashSet<string>();

            var ns = 0;
            foreach (var mz in mzs)
                ns = Math.Max(ns, mz.Model);

            var mzm = new MSetZlibStream[ns + 1, 12];

            for (int i = 0; i < mzs.Length; i++)
            {
                if (opts.VerboseMode)
                    ULogger.W("3DMC", "Processing stream {0}", i);

                var mz = mzs[i];

                int fc = file.Models[mz.Model].FaceCount;
                var vc = file.Models[mz.Model].VertexCount;

                var mzdat = new byte[mz.DecompressedLength];
                var y = 0;

                if (mz.CompressedLength == 0 && mz.DecompressedLength > 0)
                {
                    while (y < mz.DecompressedLength)
                        y += mz.Data.Read(mzdat, y, mz.DecompressedLength - y);
                }
                else
                {
                    var mzlib = new ZlibStream(mz.Data, CompressionMode.Decompress);
                    while (y < mz.DecompressedLength)
                        y += mzlib.Read(mzdat, y, mz.DecompressedLength - y);
                }

                if (mz.IsEncoded)
                {
                    if (mz.DataStream == 0)
                        mzdat = DataOperations.DeltaDecompress(mzdat, DataType.I32S, 3, fc);
                    else if (mz.DataStream <= 4 || mz.DataStream >= 10)
                        mzdat = DataOperations.DeltaDecompress(mzdat, DataType.F32, 3, vc);
                    else if (mz.DataStream == 5 || mz.DataStream == 6)
                        mzdat = DataOperations.DeltaDecompress(mzdat, DataType.F32, 2, vc);
                }

                mz.DataProcessed = mzdat;
                mzm[mz.Model, mz.DataStream] = mz;
            }

            if (file.Models.Length <= 0)
                yield break;

            for (int i = 0; i < ns + 1; i++)
            {
                if (opts.VerboseMode)
                    ULogger.W("3DMC", "Processing model {0}", i);

                var mz_tris =     mzm[i, 0];
                var mz_verts =    mzm[i, 1];
                var mz_norms =    mzm[i, 2];
                var mz_binorms =  mzm[i, 3];
                var mz_tangents = mzm[i, 4];
                var mz_sts =      mzm[i, 5];
                var mz_sts3 =     mzm[i, 6];
                var mz_colors =   mzm[i, 7];
                //var mz_weights =  mzm[i, 8];
                //var mz_matidxs =  mzm[i, 9];
                var mz_verts2 =   mzm[i, 10];
                var mz_norms2 =   mzm[i, 11];

                int fc = file.Models[mz_tris.Model].FaceCount;
                var vc = file.Models[mz_tris.Model].VertexCount;

                // ...
                var l_tris =     mz_tris.DataProcessed != null ? new ThreeDFace[fc] : null;
                var l_verts =    mz_verts.DataProcessed != null ? new ThreeDVertex[vc] : null;
                var l_norms =    mz_norms.DataProcessed != null ? new ThreeDNormal[vc] : null;
                var l_binorms =  mz_binorms.DataProcessed != null ? new ThreeDBinormal[vc] : null;
                var l_tangents = mz_tangents.DataProcessed != null ? new ThreeDTangent[vc] : null;
                var l_sts =      mz_sts.DataProcessed != null ? new ThreeDTextureCoordinate[vc] : null;
                var l_sts3 =     mz_sts3.DataProcessed != null ? new ThreeDTextureCoordinate[vc] : null;
                var l_colors =   mz_colors.DataProcessed != null ? new ThreeDColor[vc] : null;
                //var l_weights =  mz_matidxs.DataProcessed != null ? new ThreeDBoneWeight[] : null;
                //var l_matidxs =  mz_weights.DataProcessed != null ? new ThreeDBoneMaterials[] : null;
                var l_verts2 =   mz_verts2.DataProcessed != null ? new ThreeDVertex[vc] : null;
                var l_norms2 =   mz_norms2.DataProcessed != null ? new ThreeDNormal[vc] : null;

                if (l_tris != null)
                    for (int j = 0; j < fc; j++)
                    {
                        int index = j * 12;

                        var f_v1 = BitConverter.ToInt32(mz_tris.DataProcessed, index + 0);
                        var f_v2 = BitConverter.ToInt32(mz_tris.DataProcessed, index + 4);
                        var f_v3 = BitConverter.ToInt32(mz_tris.DataProcessed, index + 8);

                        //l_tris.Add(new ThreeDFace(f_v1, f_v2, f_v3));
                        l_tris[j] = new ThreeDFace(f_v1, f_v2, f_v3);
                    }

                for (int j = 0; j < vc; j++)
                {
                    int index1 = j * 12;
                    int index2 = j * 8;
                    int index3 = j * 4;

                    if (l_verts != null)
                    {
                        var v_x = BitConverter.ToSingle(mz_verts.DataProcessed, index1 + 0);
                        var v_y = BitConverter.ToSingle(mz_verts.DataProcessed, index1 + 4);
                        var v_z = BitConverter.ToSingle(mz_verts.DataProcessed, index1 + 8);

                        //l_verts.Add(new ThreeDVertex(v_x, v_y, v_z));
                        l_verts[j] = new ThreeDVertex(v_x, v_y, v_z);
                    }

                    if (l_norms != null)
                    {
                        var n_x = BitConverter.ToSingle(mz_norms.DataProcessed, index1 + 0);
                        var n_y = BitConverter.ToSingle(mz_norms.DataProcessed, index1 + 4);
                        var n_z = BitConverter.ToSingle(mz_norms.DataProcessed, index1 + 8);

                        //l_norms.Add(new ThreeDNormal(n_x, n_y, n_z));
                        l_norms[j] = new ThreeDNormal(n_x, n_y, n_z);
                    }

                    if (l_binorms != null)
                    {
                        var b_x = BitConverter.ToSingle(mz_binorms.DataProcessed, index1 + 0);
                        var b_y = BitConverter.ToSingle(mz_binorms.DataProcessed, index1 + 4);
                        var b_z = BitConverter.ToSingle(mz_binorms.DataProcessed, index1 + 8);

                        //l_binorms.Add(new ThreeDBinormal(b_x, b_y, b_z));
                        l_binorms[j] = new ThreeDBinormal(b_x, b_y, b_z);
                    }

                    if (l_tangents != null)
                    {
                        var t_x = BitConverter.ToSingle(mz_tangents.DataProcessed, index1 + 0);
                        var t_y = BitConverter.ToSingle(mz_tangents.DataProcessed, index1 + 4);
                        var t_z = BitConverter.ToSingle(mz_tangents.DataProcessed, index1 + 8);

                        //l_tangents.Add(new ThreeDTangent(t_x, t_y, t_z));
                        l_tangents[j] = new ThreeDTangent(t_x, t_y, t_z);
                    }

                    if (l_sts != null)
                    {
                        var t_u = BitConverter.ToSingle(mz_sts.DataProcessed, index2 + 0);
                        var t_v = 1F - BitConverter.ToSingle(mz_sts.DataProcessed, index2 + 4);

                        //l_sts.Add(new ThreeDTextureCoordinate(t_u, t_v));
                        l_sts[j] = new ThreeDTextureCoordinate(t_u, t_v);
                    }

                    if (l_sts3 != null)
                    {
                        var t_u = BitConverter.ToSingle(mz_sts3.DataProcessed, index2 + 0);
                        var t_v = 1F - BitConverter.ToSingle(mz_sts3.DataProcessed, index2 + 4);

                        //l_sts3.Add(new ThreeDTextureCoordinate(t_u, t_v));
                        l_sts3[j] = new ThreeDTextureCoordinate(t_u, t_v);
                    }

                    if (l_colors != null)
                    {
                        var c_r = mz_colors.DataProcessed[index3 + 0];
                        var c_g = mz_colors.DataProcessed[index3 + 1];
                        var c_b = mz_colors.DataProcessed[index3 + 2];
                        var c_a = mz_colors.DataProcessed[index3 + 3];

                        //l_colors.Add(new ThreeDColor(c_r, c_g, c_b, c_a));
                        l_colors[j] = new ThreeDColor(c_r, c_g, c_b, c_a);
                    }

                    if (l_verts2 != null)
                    {
                        var v_x = BitConverter.ToSingle(mz_verts2.DataProcessed, index1 + 0);
                        var v_y = BitConverter.ToSingle(mz_verts2.DataProcessed, index1 + 4);
                        var v_z = BitConverter.ToSingle(mz_verts2.DataProcessed, index1 + 8);

                        //l_verts2.Add(new ThreeDVertex(v_x, v_y, v_z));
                        l_verts2[j] = new ThreeDVertex(v_x, v_y, v_z);
                    }

                    if (l_norms2 != null)
                    {
                        var n_x = BitConverter.ToSingle(mz_norms2.DataProcessed, index1 + 0);
                        var n_y = BitConverter.ToSingle(mz_norms2.DataProcessed, index1 + 4);
                        var n_z = BitConverter.ToSingle(mz_norms2.DataProcessed, index1 + 8);

                        //l_norms2.Add(new ThreeDNormal(n_x, n_y, n_z));
                        l_norms2[j] = new ThreeDNormal(n_x, n_y, n_z);
                    }
                }

                var mname = this.GenerateUniqueName(mns, mz_tris.ModelName);
                mns.Add(mname);
                var m3d = new ThreeDModel
                {
                    ModelName = mname,
                    VertexCount = vc,
                    FaceCount = fc,

                    Faces = l_tris != null ? Array.AsReadOnly(l_tris) : null,
                    Vertices = l_verts != null ? Array.AsReadOnly(l_verts) : null,
                    Normals = l_norms != null ? Array.AsReadOnly(l_norms) : null,
                    Binormals = l_binorms != null ? Array.AsReadOnly(l_binorms) : null,
                    Tangents = l_tangents != null ? Array.AsReadOnly(l_tangents) : null,
                    TextureCoordinates = l_sts != null ? Array.AsReadOnly(l_sts) : null,
                    TextureCoordinates3 = l_sts3 != null ? Array.AsReadOnly(l_sts3) : null,
                    Colors = l_colors != null ? Array.AsReadOnly(l_colors) : null,
                    //
                    //
                    Vertices2 = l_verts2 != null ? Array.AsReadOnly(l_verts2) : null,
                    Normals2 = l_norms2 != null ? Array.AsReadOnly(l_norms2) : null
                };
                yield return m3d;
            }
        }

        private string GenerateUniqueName(HashSet<string> sofar, string mdlname)
        {
            var name = mdlname;
            var namec = name.ToCharArray();
            var i = 0;
            for (i = 0; i < namec.Length; i++)
                if (Array.IndexOf(LegalAlphabet, namec[i]) == -1)
                    namec[i] = '_';
            name = new string(namec);

            i = 0;
            while (sofar.Contains(name))
                name = string.Concat(mdlname, "_", i++.ToString("00"));

            return name;
        }
    }
}
