using System.Collections.ObjectModel;

namespace JohnCena.Mset.Data.ThreeD
{
    internal struct ThreeDModel
    {
        public string ModelName { get; set; }

        public int VertexCount { get; set; }

        public int FaceCount { get; set; }

        public ReadOnlyCollection<ThreeDVertex> Vertices { get; set; }
        public ReadOnlyCollection<ThreeDFace> Faces { get; set; }
        public ReadOnlyCollection<ThreeDNormal> Normals { get; set; }
        public ReadOnlyCollection<ThreeDBinormal> Binormals { get; set; }
        public ReadOnlyCollection<ThreeDTangent> Tangents { get; set; }
        public ReadOnlyCollection<ThreeDTextureCoordinate> TextureCoordinates { get; set; }
        public ReadOnlyCollection<ThreeDTextureCoordinate> TextureCoordinates3 { get; set; }
        public ReadOnlyCollection<ThreeDColor> Colors { get; set; }
        public ReadOnlyCollection<ThreeDVertex> Vertices2 { get; set; }
        public ReadOnlyCollection<ThreeDNormal> Normals2 { get; set; }
    }
}
