using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using JohnCena.Mset.Data.ThreeD;
using JohnCena.Mset.IO;

namespace JohnCena.Mset.ModelIO.Writers
{
    internal class RawDumper
    {
        public void WriteRawMesh(ThreeDModel m3d, FileInfo msetf, FileInfo m3drf, Encoding enc)
        {
            using (var fs = m3drf.Create())
            using (var tw = new FormattedStreamWriter(fs, enc, 4096, false, CultureInfo.InvariantCulture))
            using (var xw = new XmlTextWriter(tw))
            {
                xw.Formatting = Formatting.Indented;
                xw.WriteStartDocument();

                var sb = new StringBuilder();
                sb.AppendLine()
                    .AppendLine("3D Model imported from MSET")
                    .AppendLine("Using John Cena's MSET Converter")
                    .AppendFormat("Converter version: {0}", Program.GetAssemblyVersion()).AppendLine()
                    .AppendLine()
                    .AppendLine("Mesh information:")
                    .AppendFormat("Vertex count: {0}", m3d.VertexCount).AppendLine()
                    .AppendFormat("Face count: {0}", m3d.FaceCount).AppendLine();

                xw.WriteComment(sb.ToString());
                xw.WriteStartElement("3dmesh");

                xw.WriteStartElement("mesh-info");

                xw.WriteStartElement("model-name");
                xw.WriteStartAttribute("value");
                xw.WriteValue(m3d.ModelName);
                xw.WriteEndAttribute();
                xw.WriteEndElement();

                xw.WriteStartElement("source-file");
                xw.WriteStartAttribute("value");
                xw.WriteValue(msetf.Name);
                xw.WriteEndAttribute();
                xw.WriteEndElement();

                xw.WriteStartElement("vertex-count");
                xw.WriteStartAttribute("value");
                xw.WriteValue(m3d.VertexCount);
                xw.WriteEndAttribute();
                xw.WriteEndElement();

                xw.WriteStartElement("face-count");
                xw.WriteStartAttribute("value");
                xw.WriteValue(m3d.FaceCount);
                xw.WriteEndAttribute();
                xw.WriteEndElement();

                xw.WriteStartElement("data-contained");

                if (m3d.Vertices != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("vertices");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Faces != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("faces");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Normals != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("normals");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Binormals != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("binormals");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Tangents != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("tangents");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.TextureCoordinates != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("texture-coordinates");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Colors != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("vertex-colors");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.TextureCoordinates3 != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("texture-coordinates-3");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Vertices2 != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("vertices-2");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                if (m3d.Normals2 != null)
                {
                    xw.WriteStartElement("data");
                    xw.WriteStartAttribute("type");
                    xw.WriteValue("normals-2");
                    xw.WriteEndAttribute();
                    xw.WriteEndElement();
                }

                xw.WriteEndElement(); // data-contained

                xw.WriteEndElement(); // mesh-info

                xw.WriteStartElement("mesh-data");

                if (m3d.Vertices != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("vertices");
                    xw.WriteEndAttribute();

                    foreach (var vertex in m3d.Vertices)
                    {
                        xw.WriteStartElement("vertex");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(vertex.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(vertex.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(vertex.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // vertex
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Faces != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("faces");
                    xw.WriteEndAttribute();

                    foreach (var face in m3d.Faces)
                    {
                        xw.WriteStartElement("face");

                        xw.WriteStartAttribute("v1");
                        xw.WriteValue(face.Vertex1);
                        xw.WriteEndAttribute(); // v1

                        xw.WriteStartAttribute("v2");
                        xw.WriteValue(face.Vertex2);
                        xw.WriteEndAttribute(); // v2

                        xw.WriteStartAttribute("v3");
                        xw.WriteValue(face.Vertex3);
                        xw.WriteEndAttribute(); // v3

                        xw.WriteEndElement(); // face
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Normals != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("normals");
                    xw.WriteEndAttribute();

                    foreach (var normal in m3d.Normals)
                    {
                        xw.WriteStartElement("normal");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(normal.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(normal.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(normal.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // normal
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Binormals != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("binormals");
                    xw.WriteEndAttribute();

                    foreach (var binormal in m3d.Binormals)
                    {
                        xw.WriteStartElement("binormal");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(binormal.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(binormal.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(binormal.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // binormal
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Tangents != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("tangents");
                    xw.WriteEndAttribute();

                    foreach (var tangent in m3d.Tangents)
                    {
                        xw.WriteStartElement("tangent");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(tangent.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(tangent.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(tangent.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // tangent
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.TextureCoordinates != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("texture-coordinates");
                    xw.WriteEndAttribute();

                    foreach (var texcoord in m3d.TextureCoordinates)
                    {
                        xw.WriteStartElement("texcoord");

                        xw.WriteStartAttribute("u");
                        xw.WriteValue(texcoord.U.ToString("0.000000"));
                        xw.WriteEndAttribute(); // u

                        xw.WriteStartAttribute("v");
                        xw.WriteValue(texcoord.V.ToString("0.000000"));
                        xw.WriteEndAttribute(); // v

                        xw.WriteEndElement(); // texcoord
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Colors != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("colors");
                    xw.WriteEndAttribute();

                    foreach (var color in m3d.Colors)
                    {
                        xw.WriteStartElement("color");

                        xw.WriteStartAttribute("r");
                        xw.WriteValue(color.R);
                        xw.WriteEndAttribute(); // r

                        xw.WriteStartAttribute("g");
                        xw.WriteValue(color.G);
                        xw.WriteEndAttribute(); // g

                        xw.WriteStartAttribute("b");
                        xw.WriteValue(color.B);
                        xw.WriteEndAttribute(); // b

                        xw.WriteStartAttribute("a");
                        xw.WriteValue(color.A);
                        xw.WriteEndAttribute(); // a

                        xw.WriteEndElement(); // color
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.TextureCoordinates3 != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("texture-coordinates-3");
                    xw.WriteEndAttribute();

                    foreach (var texcoord3 in m3d.TextureCoordinates3)
                    {
                        xw.WriteStartElement("texcoord3");

                        xw.WriteStartAttribute("u");
                        xw.WriteValue(texcoord3.U.ToString("0.000000"));
                        xw.WriteEndAttribute(); // u

                        xw.WriteStartAttribute("v");
                        xw.WriteValue(texcoord3.V.ToString("0.000000"));
                        xw.WriteEndAttribute(); // v

                        xw.WriteEndElement(); // texcoord3
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Vertices2 != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("vertices-2");
                    xw.WriteEndAttribute();

                    foreach (var vertex2 in m3d.Vertices2)
                    {
                        xw.WriteStartElement("vertex2");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(vertex2.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(vertex2.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(vertex2.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // vertex2
                    }

                    xw.WriteEndElement(); // data
                }

                if (m3d.Normals2 != null)
                {
                    xw.WriteStartElement("data");

                    xw.WriteStartAttribute("type");
                    xw.WriteValue("normals-2");
                    xw.WriteEndAttribute();

                    foreach (var normal2 in m3d.Normals2)
                    {
                        xw.WriteStartElement("normal2");

                        xw.WriteStartAttribute("x");
                        xw.WriteValue(normal2.X.ToString("0.000000"));
                        xw.WriteEndAttribute(); // x

                        xw.WriteStartAttribute("y");
                        xw.WriteValue(normal2.Y.ToString("0.000000"));
                        xw.WriteEndAttribute(); // y

                        xw.WriteStartAttribute("z");
                        xw.WriteValue(normal2.Z.ToString("0.000000"));
                        xw.WriteEndAttribute(); // z

                        xw.WriteEndElement(); // normal2
                    }

                    xw.WriteEndElement(); // data
                }

                xw.WriteEndElement(); //mesh-data

                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Flush();
            }
        }
    }
}
