using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JohnCena.AdaptedLogger;
using JohnCena.Mset.Data.ThreeD;
using JohnCena.Mset.IO;

namespace JohnCena.Mset.ModelIO.Writers
{
    internal class ObjModelWriter : IModelWriter
    {
        public int VersionID { get { return 3; } }
        public string WriterID { get { return "OBJ"; } }
        public string FileExtension { get { return ".obj"; } }
        public Dictionary<string, string> Options { get { return this.opts; } }

        private Stream ostream;
        private TextWriter tw;
        private FileInfo fi;
        private Dictionary<string, string> opts;

        public ObjModelWriter()
        {
            this.opts = new Dictionary<string, string>();
        }

        public void WriteModel(ThreeDModel m3d, Stream stream)
        {
            this.ostream = stream;
            this.tw = new FormattedStreamWriter(this.ostream, new UTF8Encoding(false), 4096, false, CultureInfo.InvariantCulture);
            this.tw.NewLine = "\n";
            this.WriteInternal(m3d);
        }

        public void WriteModel(ThreeDModel m3d, FileInfo file)
        {
            this.fi = file;
            using (var fs = file.Create())
                this.WriteModel(m3d, fs);
        }

        public void WriteModel(ThreeDModel m3d, string filename)
        {
            this.WriteModel(m3d, new FileInfo(filename));
        }

        public void Dispose()
        {
            this.ostream.Dispose();
        }

        private void WriteInternal(ThreeDModel m3d)
        {
            var opts = Program.Options;
            var usemtl = this.opts.ContainsKey("usemtl") && this.opts["usemtl"] == "true";
            if (usemtl && !opts.IsColorDefined)
                ULogger.W("OBJWRITE", "usemtl set to true but no override color defined; ignoring...");
            usemtl = usemtl && opts.IsColorDefined;
            if (!usemtl && opts.IsColorDefined && opts.OverrideColor.A != 255)
                ULogger.W("OBJWRITE", "usemtl is not set to true, but override color has Alpha different from 255 -- it won't be written!");

            this.tw.WriteLine("# Converted using MSET Converter by John Cena of PTF");
            this.tw.WriteLine("# mset Version: {0}", Program.GetAssemblyVersion());
            this.tw.WriteLine("# mset-obj Version: {0}", this.VersionID);
            this.tw.WriteLine("# ");
            this.tw.WriteLine("# Model Summary: ");
            this.tw.WriteLine("# Name: {0}", m3d.ModelName);
            this.tw.WriteLine("# Vertex Count: {0:#,##0} ", m3d.VertexCount);
            this.tw.WriteLine("# Face Count: {0:#,##0} ", m3d.FaceCount);
            this.tw.WriteLine("# ");
            this.tw.WriteLine("# Writer options:");
            foreach (var kvp in this.opts)
                this.tw.WriteLine("# {0}={1}", kvp.Key, kvp.Value);
            this.tw.WriteLine();
            
            var hasclr = m3d.Colors != null;
            var hasnorm = m3d.Normals != null;
            var hastex = m3d.TextureCoordinates != null;
            var mtlfn = (string)null;
            if (this.fi != null && usemtl)
            {
                mtlfn = this.fi.Name.Substring(0, this.fi.Name.Length - this.fi.Extension.Length);
                this.tw.WriteLine("mtllib {0}.mtl", mtlfn);
            }

            this.tw.WriteLine("o Mset.1");
            this.tw.WriteLine();

            this.tw.WriteLine("# Vertices");
            //foreach (var vert in m3d.Vertices)
            for (int i = 0; i < m3d.VertexCount; i++)
            {
                var vert = m3d.Vertices[i];
                var clr = hasclr && !usemtl ? m3d.Colors[i] : default(ThreeDColor);

                if (hasclr && !usemtl)
                    this.tw.WriteLine("v {0:0.000000} {1:0.000000} {2:0.000000} {3:0.000000} {4:0.000000} {5:0.000000}", vert.X, vert.Y, vert.Z, clr.RFloat, clr.GFloat, clr.BFloat);
                else
                    this.tw.WriteLine("v {0:0.000000} {1:0.000000} {2:0.000000}", vert.X, vert.Y, vert.Z);
            }
            this.tw.WriteLine("");

            if (hasnorm)
            {
                this.tw.WriteLine("# Normals");
                foreach (var norm in m3d.Normals)
                {
                    this.tw.WriteLine("vn {0:0.000000} {1:0.000000} {2:0.000000}", norm.X, norm.Y, norm.Z);
                }
                this.tw.WriteLine("");
            }

            if (hastex)
            {
                this.tw.WriteLine("# Texture Coordinates");
                foreach (var texcoord in m3d.TextureCoordinates)
                {
                    this.tw.WriteLine("vt {0:0.000000} {1:0.000000}", texcoord.U, texcoord.V);
                }
                this.tw.WriteLine("");
            }
            
            this.tw.WriteLine("# Faces");
            if (this.fi != null && usemtl)
                this.tw.WriteLine("usemtl JCMset");
            //foreach (var face in m3d.Faces)
            for (int i = 0; i < m3d.FaceCount; i++)
            {
                var face = m3d.Faces[i];

                if (hastex && hasnorm)
                    this.tw.WriteLine("f {0:0}/{0:0}/{0:0} {1:0}/{1:0}/{1:0} {2:0}/{2:0}/{2:0}", face.Vertex1 + 1, face.Vertex2 + 1, face.Vertex3 + 1);
                else if (hastex && !hasnorm)
                    this.tw.WriteLine("f {0:0}/{0:0} {1:0}/{1:0} {2:0}/{2:0}", face.Vertex1 + 1, face.Vertex2 + 1, face.Vertex3 + 1);
                else if (!hastex && hasnorm)
                    this.tw.WriteLine("f {0:0}//{0:0} {1:0}//{1:0} {2:0}//{2:0}", face.Vertex1 + 1, face.Vertex2 + 1, face.Vertex3 + 1);
                else
                    this.tw.WriteLine("f {0:0} {1:0} {2:0}", face.Vertex1 + 1, face.Vertex2 + 1, face.Vertex3 + 1);
            }
            this.tw.WriteLine("");

            this.tw.Flush();

            if (usemtl)
                this.WriteMtl(m3d, mtlfn);
        }

        private void WriteMtl(ThreeDModel m3d, string mtlfn)
        {
            var opts = Program.Options;
            var clr = opts.OverrideColor;
            var dn = this.fi.Directory.FullName;
            var p = Path.Combine(dn, string.Concat(mtlfn, ".mtl"));
            var mtlfi = new FileInfo(p);

            using (var fs = mtlfi.Create())
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                sw.NewLine = "\n";

                sw.WriteLine("# Generated using MSET Converter by John Cena of PTF");
                sw.WriteLine("# mset Version: {0}", Program.GetAssemblyVersion());
                sw.WriteLine("# mset-obj Version: {0}", this.VersionID);
                sw.WriteLine();
                sw.WriteLine("# Default MSET material");
                sw.WriteLine("newmtl JCMSet");
                sw.WriteLine("Kd {0:0.000000} {1:0.000000} {2:0.000000}", clr.RFloat, clr.GFloat, clr.BFloat);
                sw.WriteLine("d  {0:0.000000}", clr.AFloat);
                sw.WriteLine("Tr {0:0.000000}", 1F - clr.AFloat);
                sw.WriteLine();

                sw.Flush();
            }

            if (opts.VerboseMode)
                ULogger.W("OBJMTL", "Written MTL: {0}", mtlfi.FullName);
        }
    }
}
