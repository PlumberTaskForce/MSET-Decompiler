using System;
using System.Collections.Generic;
using System.IO;
using JohnCena.Mset.Data.ThreeD;

namespace JohnCena.Mset.ModelIO.Writers
{
    internal interface IModelWriter : IDisposable
    {
        int VersionID { get; }
        string WriterID { get; }
        string FileExtension { get; }
        Dictionary<string, string> Options { get; }

        void WriteModel(ThreeDModel m3d, string filename);
        void WriteModel(ThreeDModel m3d, FileInfo file);
        void WriteModel(ThreeDModel m3d, Stream stream);
    }
}
