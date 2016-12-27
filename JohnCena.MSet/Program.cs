using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JohnCena.AdaptedLogger;
using JohnCena.Mset.Data;
using JohnCena.Mset.Data.ThreeD;
using JohnCena.Mset.ModelIO;
using JohnCena.Mset.ModelIO.Writers;

namespace JohnCena.Mset
{
    internal static class Program
    {
        internal static CommandLine Options { get; private set; }

        private static Version v = null;

        /*
         * COMMANDLINE OPTIONS
         *
         * Extraction options:
         *   -e:<encoder>
         *   Specifies output encoder. <encoder> must be a valid encoder. For available encoders, see readme.
         *   Default value: OBJ
         *
         *   -d:<path>
         *   Specifies output directory. <path> must be a valid directory path.
         *   Default value: <null> (outputs to source files' directory)
         *
         *   -l
         *   Lists models in a file then exists.
         *
         *   -m:<model_name>
         *   Specifies extracting a specific model from input file(s). <model_name> is a model to extract.
         *   Default value: <null> (extracts all models)
         *
         *   -c:<color>
         *   Overrides vertex colors on resulting mesh. <color> is an unsigned 32-bit integer containing ARGB information.
         *   Default value: <null> (does not override colors)
         *
         *   -o:<options>
         *   Specifies selected encoder's options. <options> is a string of comma-separated key-value pairs. For examples and available encoder options, see readme.
         *   Default value: <null> (use default options)
         *
         *   -r
         *   Creates raw mesh dumps alongside output files. These are UTF-8-encoded text files describing raw input data.
         *
         * Program options:
         *   -v
         *   Enables verbose logging to console.
         *
         *   -v:<path>
         *   Enables logging to file. Supersedes -v. <path> must be a valid file path.
         */
        internal static void Main(string[] args)
        {
            ULogger.D(Debugger.IsAttached);
            ULogger.R(Console.Out);

            ULogger.W("MSET2", "Initializing");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var cmdl = ParseCommandLine(args);
            Options = cmdl;

            if (cmdl.VerboseMode)
            {
                if (cmdl.VerboseModeLogFile != null)
                {
                    var tw = new StreamWriter(cmdl.VerboseModeLogFile.Create(), new UTF8Encoding(false));
                    ULogger.R(tw);
                }

                ULogger.W("APP", "Commandline options:");
                ULogger.W("APP", "  Encoder: {0} (ver. {1})", cmdl.ModelWriter.WriterID, cmdl.ModelWriter.VersionID);
                ULogger.W("APP", "  Output path: {0}", cmdl.OutputDirectory != null ? cmdl.OutputDirectory.FullName : "<null>");
                ULogger.W("APP", "  Just list models: {0}", cmdl.ListModels ? "Yes" : "No");
                ULogger.W("APP", "  Model name: {0}", cmdl.ModelName != null ? cmdl.ModelName : "<null>");
                ULogger.W("APP", "  Override color: {0}", cmdl.IsColorDefined ? string.Format("Yes (RGBA #{0:X2}{1:X2}{2:X2}{3:X2})", cmdl.OverrideColor.R, cmdl.OverrideColor.G, cmdl.OverrideColor.B, cmdl.OverrideColor.A) : "No");
                ULogger.W("APP", "  Encoder options specified: {0}", cmdl.ModelWriterOptions.Count);
                ULogger.W("APP", "  Create raw meshes: {0}", cmdl.CreateRawMeshes ? "Yes" : "No");
                ULogger.W("APP", "  Verbose mode: {0}", cmdl.VerboseMode ? "Yes" : "No");
                ULogger.W("APP", "  Log file: {0}", cmdl.VerboseModeLogFile != null ? cmdl.VerboseModeLogFile.FullName : "<null>");
                ULogger.N("APP");
                ULogger.W("APP", "Invalid arguments specified: {0}", cmdl.HasInvalidArguments ? "Yes" : "No");
                ULogger.N("APP");
                ULogger.W("APP", "Input files ({0}):", cmdl.InputFiles.Count());
                foreach (var infile in cmdl.InputFiles)
                    ULogger.W("APP", "'{0}'", infile.FullName);
                ULogger.N("APP");
            }

            if (cmdl.HasInvalidArguments && cmdl.ArgumentException != null)
            {
                ULogger.X("APP", cmdl.ArgumentException);
                ULogger.N("APP");
            }

            if (cmdl.HelpMode || cmdl.HasInvalidArguments)
            {
                PrintUse();
                ULogger.Q();
                return;
            }

            var msetrdr = new MSetReader();
            var msetdmp = new RawDumper();
            foreach (var kvp in cmdl.ModelWriterOptions)
                cmdl.ModelWriter.Options.Add(kvp.Key, kvp.Value);
            var m3dw = cmdl.ModelWriter;
            var utf8 = new UTF8Encoding(false);
            try
            {
                foreach (var msetf in cmdl.InputFiles)
                {
                    if (cmdl.VerboseMode)
                        ULogger.W("MSETLDR", "Loading '{0}'", msetf.FullName);

                    var mset = msetrdr.Load(msetf);
                    var m3ds = msetrdr.Create3DModels(mset).ToList();

                    if (cmdl.VerboseMode)
                        ULogger.W("MSETLDR", "Loaded {0} models", m3ds.Count);

                    foreach (var m3d in m3ds)
                    {
                        ULogger.W("M3D", "Model '{0}' in file '{1}'", m3d.ModelName, msetf.FullName);
                        if (cmdl.ListModels)
                            continue;

                        if (cmdl.ModelName != null && m3d.ModelName != cmdl.ModelName)
                            continue;

                        ULogger.W("M3DWRTR", "Extracting '{0}'", m3d.ModelName);
                        var opath = cmdl.OutputDirectory != null ? cmdl.OutputDirectory.FullName : msetf.Directory.FullName;
                        //var msetn = msetf.Name.Substring(0, msetf.Name.Length - msetf.Extension.Length);
                        var msetn = m3d.ModelName;
                        msetn = string.Concat(msetn, m3dw.FileExtension);
                        opath = Path.Combine(opath, msetn);
                        var m3df = new FileInfo(opath);

                        m3dw.WriteModel(m3d, m3df);

                        if (cmdl.CreateRawMeshes)
                        {
                            if (cmdl.VerboseMode)
                                ULogger.W("M3D RAW", "Creating raw dump");

                            opath = string.Concat(opath, ".raw_3d");
                            var m3drf = new FileInfo(opath);

                            msetdmp.WriteRawMesh(m3d, msetf, m3drf, utf8);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ULogger.W("APP ERROR", "CRITICAL: ERROR WHILE UNPACKING MSET");
                ULogger.X("APP ERROR", ex);
            }

            ULogger.W("MSET2", "Conversion completed");
            ULogger.Q();

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject != null && e.ExceptionObject is Exception ? (Exception)e.ExceptionObject : null;

            ULogger.W("ERR", "An error has occured within the application");
            if (ex != null)
                ULogger.X("ERR", ex);
            else
                ULogger.W("ERR", "FATAL: No details are avaialble");
        }

        internal static void PrintUse()
        {
            ULogger.W("APP", "Usage: mset2 <arguments...> <files>");
            ULogger.N("APP");
            ULogger.W("APP", "Extraction options:");
            ULogger.W("APP", "  -e:<encoder>");
            ULogger.W("APP", "  Specifies output encoder. <encoder> must be a valid encoder. For available encoders, see readme.");
            ULogger.W("APP", "  Default value: OBJ");
            ULogger.N("APP");
            ULogger.W("APP", "  -d:<path>");
            ULogger.W("APP", "  Specifies output directory. <path> must be a valid directory path.");
            ULogger.W("APP", "  Default value: <null> (outputs to source files' directory)");
            ULogger.N("APP");
            ULogger.W("APP", "  -l");
            ULogger.W("APP", "  Lists models in a file then exists.");
            ULogger.N("APP");
            ULogger.W("APP", "  -m:<model_name>");
            ULogger.W("APP", "  Specifies extracting a specific model from input file(s). <model_name> is a model to extract.");
            ULogger.W("APP", "  Default value: <null> (extracts all models)");
            ULogger.N("APP");
            ULogger.W("APP", "  -c:<color>");
            ULogger.W("APP", "  Overrides vertex colors on resulting mesh. <color> is an unsigned 32-bit integer containing ARGB information.");
            ULogger.W("APP", "  Default value: <null> (does not override colors)");
            ULogger.N("APP");
            ULogger.W("APP", "  -o:<options>");
            ULogger.W("APP", "  Specifies selected encoder's options. <options> is a string of comma-separated key-value pairs. For examples and available encoder options, see readme.");
            ULogger.W("APP", "  Default value: <null> (use default options)");
            ULogger.N("APP");
            ULogger.W("APP", "  -r");
            ULogger.W("APP", "  Creates raw mesh dumps alongside output files. These are UTF-8-encoded text files describing raw input data.");
            ULogger.N("APP");
            ULogger.W("APP", "Program options:");
            ULogger.W("APP", "  -v");
            ULogger.W("APP", "  Enables verbose logging to console.");
            ULogger.N("APP");
            ULogger.W("APP", "  -v:<path>");
            ULogger.W("APP", "  Enables logging to file. Supersedes -v. <path> must be a valid file path.");
            ULogger.N("APP");
            ULogger.W("APP", "  -help");
            ULogger.W("APP", "  Prints this message and quits.");
            ULogger.N("APP");
        }

        internal static Version GetAssemblyVersion()
        {
            if (v != null)
                return v;

            var a = Assembly.GetExecutingAssembly();
            var an = a.GetName();
            v = an.Version;
            return v;
        }

        internal static CommandLine ParseCommandLine(string[] args)
        {
            if (args.Contains("-help") || args.Length == 0)
                return new CommandLine
                {
                    HelpMode = true
                };

            var mws = RegisterWriters();

            var mw = mws.First(xmw => xmw.WriterID == "OBJ");
            var od = (DirectoryInfo)null;
            var lm = false;
            var mn = (string)null;
            var cl = default(ThreeDColor);
            var cd = false;
            var co = new Dictionary<string, string>();
            var cr = false;
            var vb = false;
            var vf = (FileInfo)null;

            var fs = new List<FileInfo>();

            var vl = false;
            var ve = new Exception();

            try
            {
                ve = null;

                foreach (var arg in args)
                {
                    if (arg.StartsWith("-"))
                    {
                        var xarg = arg.Substring(1);
                        if (xarg.Length <= 0)
                        {
                            vl = true;
                            ve = new ArgumentException("Argument name has to be at least one character long");
                            break;
                        }

                        var xarg_i = xarg.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (xarg_i.Length < 1)
                        {
                            vl = true;
                            ve = new ArgumentException("Argument name has to be at least one character long");
                            break;
                        }

                        var xa = xarg_i[0].ToLower();
                        var xr = xarg_i.Length > 1 ? xarg_i[1] : null;
                        if (xa == "e")
                        {
                            if (xarg_i.Length < 2)
                            {
                                vl = true;
                                ve = new ArgumentException("Argument e requires an parameter");
                                break;
                            }

                            xr = xr.ToUpper();
                            mw = mws.First(xmw => xmw.WriterID == xr);
                            if (mw == null)
                            {
                                vl = true;
                                ve = new ArgumentException("Invalid encoder specified");
                                break;
                            }
                        }
                        else if (xa == "d")
                        {
                            if (xarg_i.Length < 2)
                            {
                                vl = true;
                                ve = new ArgumentException("Argument d requires an parameter");
                                break;
                            }

                            od = new DirectoryInfo(xr);
                        }
                        else if (xa == "l")
                            lm = true;
                        else if (xa == "m")
                        {
                            if (xarg_i.Length < 2)
                            {
                                vl = true;
                                ve = new ArgumentException("Argument m requires an parameter");
                                break;
                            }

                            mn = xr;
                        }
                        else if (xa == "c")
                        {
                            uint argb = 0;
                            if (xarg_i.Length < 2 || !uint.TryParse(xr, out argb))
                            {
                                vl = true;
                                ve = new ArgumentException("Argument c requires an uint parameter");
                                break;
                            }

                            byte a = (byte)(argb >> 24);
                            byte r = (byte)(argb >> 16);
                            byte g = (byte)(argb >> 8);
                            byte b = (byte)argb;

                            cl = new ThreeDColor(r, g, b, a);
                            cd = true;
                        }
                        else if (xa == "o")
                        {
                            if (xarg_i.Length < 2)
                            {
                                vl = true;
                                ve = new ArgumentException("Argument o requires an parameter");
                                break;
                            }

                            var kvps = xr.Split(',');
                            foreach (var kvp in kvps)
                            {
                                var xkvp = kvp.Split('=');
                                co.Add(xkvp[0].ToLowerInvariant(), xkvp[1].ToLowerInvariant());
                            }
                        }
                        else if (xa == "r")
                            cr = true;
                        else if (xa == "v")
                        {
                            vb = true;

                            if (xarg_i.Length == 2)
                                vf = new FileInfo(xr);
                        }
                        else
                        {
                            vl = true;
                            ve = new ArgumentException("Invalid argument", xa);
                            break;
                        }
                    }
                    else
                    {
                        var xfi = new FileInfo(arg);
                        if ((xfi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                            fs.AddRange(Crawl(new DirectoryInfo(xfi.FullName)));
                        else if (xfi.Extension.ToLowerInvariant() == ".mset")
                            fs.Add(xfi);
                    }
                }
            }
            catch (Exception ex)
            {
                vl = true;
                ve = ex;
            }
            
            return new CommandLine
            {
                HelpMode = false,
                IsColorDefined = cd,
                HasInvalidArguments = vl,
                ArgumentException = ve,

                ModelWriter = mw,
                OutputDirectory = od,
                ListModels = lm,
                ModelName = mn,
                OverrideColor = cl,
                ModelWriterOptions = co,
                CreateRawMeshes = cr,
                VerboseMode = vb,
                VerboseModeLogFile = vf,

                InputFiles = fs
            };
        }

        private static IEnumerable<FileInfo> Crawl(DirectoryInfo di)
        {
            var fis = di.GetFiles("*", SearchOption.TopDirectoryOnly);
            var dis = di.GetDirectories("*", SearchOption.TopDirectoryOnly);
            var ret = new List<FileInfo>();

            foreach (var xfi in fis)
                if (xfi.Extension.ToLowerInvariant() == ".mset")
                    ret.Add(xfi);

            foreach (var xdi in dis)
                ret.AddRange(Crawl(xdi));

            return ret;
        }

        private static IEnumerable<IModelWriter> RegisterWriters()
        {
            var type = typeof(IModelWriter);
            var a = Assembly.GetExecutingAssembly();
            var ts = a
                .GetTypes()
                .Where(xt => type.IsAssignableFrom(xt) && !xt.IsInterface);

            foreach (var t in ts)
            {
                yield return (IModelWriter)Activator.CreateInstance(t);
            }
        }
    }
}
