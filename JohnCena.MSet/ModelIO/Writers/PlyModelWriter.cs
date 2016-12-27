using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JohnCena.Mset.Data.ThreeD;
using JohnCena.Mset.IO;

namespace JohnCena.Mset.ModelIO.Writers
{
    internal class PlyModelWriter : IModelWriter
    {
        public int VersionID { get { return 5; } }
        public string WriterID { get { return "PLY"; } }
        public string FileExtension { get { return ".ply"; } }
        public Dictionary<string, string> Options { get { return this.opts; } }

        private Stream ostream;
        private TextWriter tw;
        private BinaryWriter bw;
        private FileInfo fi;
        private Dictionary<string, string> opts;

        public PlyModelWriter()
        {
            this.opts = new Dictionary<string, string>();
        }

        public void WriteModel(ThreeDModel m3d, Stream stream)
        {
            this.ostream = stream;
            this.tw = new FormattedStreamWriter(this.ostream, new ASCIIEncoding(), 4096, true, CultureInfo.InvariantCulture);
            this.bw = new BinaryWriter(this.ostream, new ASCIIEncoding(), true);
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
            var oclr = opts.OverrideColor;
            var is_ascii = this.opts.ContainsKey("fmt") && this.opts["fmt"] == "ascii";
            var ascii = is_ascii ? new ASCIIEncoding() : null;

            this.tw.WriteLine("ply");
            if (is_ascii)
                this.tw.WriteLine("format ascii 1.0");
            else
                this.tw.WriteLine("format binary_little_endian 1.0");

            var hasclr = m3d.Colors != null;
            var hasnorm = m3d.Normals != null;
            var hastex = m3d.TextureCoordinates != null;

            this.tw.WriteLine("comment Converted using MSET Converter by John Cena of PTF");
            this.tw.WriteLine("comment mset Version: {0}", Program.GetAssemblyVersion());
            this.tw.WriteLine("comment mset-ply Version: {0}", this.VersionID);
            this.tw.WriteLine("comment ");
            this.tw.WriteLine("comment Model Summary: ");
            this.tw.WriteLine("comment Name: {0}", m3d.ModelName);
            this.tw.WriteLine("comment Vertex Count: {0:#,##0} ", m3d.VertexCount);
            this.tw.WriteLine("comment Face Count: {0:#,##0} ", m3d.FaceCount);
            this.tw.WriteLine("comment ");
            this.tw.WriteLine("comment Writer options:");
            foreach (var kvp in this.opts)
                this.tw.WriteLine("comment {0}={1}", kvp.Key, kvp.Value);

            this.tw.WriteLine("element vertex {0:0}", m3d.VertexCount);
            this.tw.WriteLine("property float x");
            this.tw.WriteLine("property float y");
            this.tw.WriteLine("property float z");

            if (hasnorm)
            {
                this.tw.WriteLine("property float nx");
                this.tw.WriteLine("property float ny");
                this.tw.WriteLine("property float nz");
            }

            if (hasclr)
            {
                this.tw.WriteLine("property uchar red");
                this.tw.WriteLine("property uchar green");
                this.tw.WriteLine("property uchar blue");
                this.tw.WriteLine("property uchar alpha");
            }

            if (hastex)
            {
                this.tw.WriteLine("property float s");
                this.tw.WriteLine("property float t");
            }

            this.tw.WriteLine("element face {0:0}", m3d.FaceCount);
            this.tw.WriteLine("property list uchar int vertex_indices");

            this.tw.WriteLine("end_header");
            this.tw.Flush();

            for (int i = 0; i < m3d.VertexCount; i++)
            {
                var vert = m3d.Vertices[i];
                var norm = hasnorm ? m3d.Normals[i] : default(ThreeDNormal);
                var clr = hasclr ? (m3d.Colors[i]) : default(ThreeDColor);
                var texcoord = hastex ? m3d.TextureCoordinates[i] : default(ThreeDTextureCoordinate);

                if (is_ascii)
                {
                    this.tw.Write("{0:0.000000} {1:0.000000} {2:0.000000}", vert.X, vert.Y, vert.Z);

                    if (hasnorm)
                        this.tw.Write(" {0:0.000000} {1:0.000000} {2:0.000000}", norm.X, norm.Y, norm.Z);

                    if (hasclr && !opts.IsColorDefined)
                        this.tw.Write(" {0:0} {1:0} {2:0} {3:0}", clr.R, clr.G, clr.B, clr.A);
                    else if (hasclr)
                        this.tw.Write(" {0:0} {1:0} {2:0} {3:0}", oclr.R, oclr.G, oclr.B, oclr.A);

                    if (hastex)
                        this.tw.Write(" {0:0.000000} {1:0.000000}", texcoord.U, texcoord.V);

                    this.tw.WriteLine();
                }
                else
                {
                    this.bw.Write(vert.X);
                    this.bw.Write(vert.Y);
                    this.bw.Write(vert.Z);

                    if (hasnorm)
                    {
                        this.bw.Write(norm.X);
                        this.bw.Write(norm.Y);
                        this.bw.Write(norm.Z);
                    }

                    if (hasclr && !opts.IsColorDefined)
                    {
                        this.bw.Write(clr.R);
                        this.bw.Write(clr.G);
                        this.bw.Write(clr.B);
                        this.bw.Write(clr.A);
                    }
                    else if (hasclr)
                    {
                        this.bw.Write(oclr.R);
                        this.bw.Write(oclr.G);
                        this.bw.Write(oclr.B);
                        this.bw.Write(oclr.A);
                    }

                    if (hastex)
                    {
                        this.bw.Write(texcoord.U);
                        this.bw.Write(texcoord.V);
                    }
                }
            }

            if (is_ascii)
            {
                foreach (var face in m3d.Faces)
                {
                    this.tw.WriteLine("3 {0:0} {1:0} {2:0}", face.Vertex1, face.Vertex2, face.Vertex3);
                }
            }
            else
            {
                foreach (var face in m3d.Faces)
                {
                    this.bw.Write((byte)3);
                    this.bw.Write(face.Vertex1);
                    this.bw.Write(face.Vertex2);
                    this.bw.Write(face.Vertex3);
                }
            }

            if (is_ascii)
                this.tw.Flush();
            else
                this.bw.Flush();
        }
    }
}
