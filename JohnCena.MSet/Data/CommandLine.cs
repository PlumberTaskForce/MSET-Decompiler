using System;
using System.Collections.Generic;
using System.IO;
using JohnCena.Mset.Data.ThreeD;
using JohnCena.Mset.ModelIO.Writers;

namespace JohnCena.Mset.Data
{
    internal struct CommandLine
    {
        public IModelWriter ModelWriter { get; set; }
        public DirectoryInfo OutputDirectory { get; set; }
        public bool ListModels { get; set; }
        public string ModelName { get; set; }
        public ThreeDColor OverrideColor { get; set; }
        public Dictionary<string, string> ModelWriterOptions { get; set; }
        public bool CreateRawMeshes { get; set; }
        public bool VerboseMode { get; set; }
        public FileInfo VerboseModeLogFile { get; set; }

        public bool HelpMode { get; set; }
        public bool IsColorDefined { get; set; }
        public bool HasInvalidArguments { get; set; }
        public Exception ArgumentException { get; set; }

        public IEnumerable<FileInfo> InputFiles { get; set; }
    }
}
