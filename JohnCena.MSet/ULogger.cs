using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// code adapted from original
// found somewhere on github
// 
// thanks to original autor, 
// and so on, yadda yadda
namespace JohnCena.AdaptedLogger
{
    /// <summary>
    /// Represents the MicroLogger interface.
    /// </summary>
    public static class ULogger
    {
        private static List<TextWriter> outputs;
        private static bool debug_output;

        /// <summary>
        /// Ran when the MicroLogger is initialized.
        /// </summary>
        static ULogger()
        {
            outputs = new List<TextWriter>();
            debug_output = false;
        }

        /// <summary>
        /// Enable or disable debug message logging.
        /// </summary>
        /// <param name="d">Whether or not debug logging is to be enabled.</param>
        public static void D(bool d)
        {
            debug_output = d;
        }

        /// <summary>
        /// Registers a new log output.
        /// </summary>
        /// <param name="tw">Log output to register.</param>
        public static void R(TextWriter tw)
        {
            outputs.Add(tw);
        }

        /// <summary>
        /// Disposes of all log outputs.
        /// </summary>
        public static void Q()
        {
            foreach (var output in outputs)
            {
                output.Flush();
                //output.Close();
                output.Dispose();
            }
        }

        /// <summary>
        /// Writes an empty message with default tag.
        /// </summary>
        public static void N()
        {
            W("");
        }

        /// <summary>
        /// Writes an empty message with specified tag.
        /// </summary>
        /// <param name="tag">Message tag.</param>
        public static void N(string tag)
        {
            W(tag, "");
        }

        /// <summary>
        /// Writes a message with default tag.
        /// </summary>
        /// <param name="msg">Message to write.</param>
        public static void W(string msg)
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            var m = msg;
            var ls = C(m, "stdout");
            foreach (var output in outputs)
                foreach (var xl in ls)
                    //Console.WriteLine(xl);
                    output.WriteLine(xl);
            if (debug_output)
                foreach (var xl in ls)
                    Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a formatted message with default tag.
        /// </summary>
        /// <param name="format">Message format. See <see cref="string.Format(string, object[])"/> for documentation.</param>
        /// <param name="args">Arguments for message format.</param>
        public static void W(string format, params object[] args)
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            var m = string.Format(format, args);
            var ls = C(m, "stdout");
            foreach (var output in outputs)
                foreach (var xl in ls)
                    //Console.WriteLine(xl);
                    output.WriteLine(xl);
            if (debug_output)
                foreach (var xl in ls)
                    Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a message with specified tag.
        /// </summary>
        /// <param name="tag">Message tag.</param>
        /// <param name="msg">Message to write.</param>
        public static void W(string tag, string msg)
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            var m = msg;
            var ls = C(m, tag);
            foreach (var output in outputs)
                foreach (var xl in ls)
                    //Console.WriteLine(xl);
                    output.WriteLine(xl);
            if (debug_output)
                foreach (var xl in ls)
                    Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a formatted message with specified tag.
        /// </summary>
        /// <param name="tag">Message to tag.</param>
        /// <param name="format">Message format. See <see cref="string.Format(string, object[])"/> for documentation.</param>
        /// <param name="args">Arguments for message format.</param>
        public static void W(string tag, string format, params object[] args)
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            var m = string.Format(format, args);
            var ls = C(m, tag);
            foreach (var output in outputs)
                foreach (var xl in ls)
                    //Console.WriteLine(xl);
                    output.WriteLine(xl);
            if (debug_output)
                foreach (var xl in ls)
                    Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a message with default tag. This method will write to debug output only.
        /// </summary>
        /// <param name="msg">Message to write.</param>
        public static void D(string msg)
        {
            if (!debug_output)
                return;

            var m = msg;
            var ls = C(m, "stdout");
            foreach (var xl in ls)
                Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a formatted message with default tag. This method will write to debug output only.
        /// </summary>
        /// <param name="format">Message format. See <see cref="string.Format(string, object[])"/> for documentation.</param>
        /// <param name="args">Arguments for message format.</param>
        public static void D(string format, params object[] args)
        {
            if (!debug_output)
                return;

            var m = string.Format(format, args);
            var ls = C(m, "stdout");
            foreach (var xl in ls)
                Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a message with specified tag. This method will write to debug output only.
        /// </summary>
        /// <param name="tag">Message tag.</param>
        /// <param name="msg">Message to write.</param>
        public static void D(string tag, string msg)
        {
            if (!debug_output)
                return;

            var m = msg;
            var ls = C(m, tag);
            foreach (var xl in ls)
                Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a formatted message with specified tag. This method will write to debug output only.
        /// </summary>
        /// <param name="tag">Message to tag.</param>
        /// <param name="format">Message format. See <see cref="string.Format(string, object[])"/> for documentation.</param>
        /// <param name="args">Arguments for message format.</param>
        public static void D(string tag, string format, params object[] args)
        {
            if (!debug_output)
                return;

            var m = string.Format(format, args);
            var ls = C(m, tag);
            foreach (var xl in ls)
                Debug.WriteLine(xl);
        }

        /// <summary>
        /// Writes a formatted message with specified tag. This method will not write to debug output, will not write a newline and will write a carriage return before writing.
        /// </summary>
        /// <param name="tag">Message to tag.</param>
        /// <param name="format">Message format. See <see cref="string.Format(string, object[])"/> for documentation.</param>
        /// <param name="width">Output width.</param>
        /// <param name="args">Arguments for message format.</param>
        internal static void Z(string tag, string format, int width, params object[] args)
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            var m = string.Format(format, args);
            var s = C(m, tag).First();
            //s = string.Concat(s, new string(' ', Console.WindowWidth - s.Length));
            s = string.Concat(s, new string(' ', width - s.Length));

            foreach (var output in outputs)
                //Console.Write("\r{0}", s);
                output.Write("\r{0}", s);
            //if (debug_output)
            //    Debug.Write(string.Format("\r{0}", s));
        }

        internal static void V()
        {
            if (outputs.Count == 0 && !debug_output)
                return;

            foreach (var output in outputs)
                //Console.WriteLine();
                output.WriteLine();
            if (debug_output)
                Debug.WriteLine("");
        }

        /// <summary>
        /// This method logs and exception to all registered log outputs and debug output using default tag.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        public static void X(Exception ex)
        {
            X("stderr", ex);
        }

        /// <summary>
        /// This method logs and exception to all registered log outputs and debug output using specified tag.
        /// </summary>
        /// <param name="tag">Message tag.</param>
        /// <param name="ex">Exception to log.</param>
        public static void X(string tag, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("EXCEPTION OCCURED");
            sb.AppendFormat("Type:          {0}", ex.GetType()).AppendLine();
            sb.AppendFormat("Message:       {0}", ex.Message).AppendLine();
            //sb.AppendFormat("Target site:   {0}", ex.TargetSite).AppendLine();
            sb.AppendLine("Stack trace:");
            sb.AppendLine(ex.StackTrace);

            W(tag, sb.ToString());
        }

        private static string T(string t)
        {
            if (t.Length == 10)
                return t;
            else if (t.Length > 10)
                return t.Substring(0, 10);
            else if (t.Length < 10)
                return string.Concat(t, new string(' ', 10 - t.Length));
            return "          ";
        }

        private static IEnumerable<string> C(string m, string t)
        {
            var l = m.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var u = T(t);
            var d = DateTime.Now;
            foreach (var xl in l)
                yield return string.Format("[{0:yyyy-MM-ddTHH:mm:ss}] [{1}] {2}", d, u, xl);
        }

        private static string F(string l, string t)
        {
            return string.Format("[{0:yyyy-MM-ddTHH:mm:ss}] [{1}] {2}");
        }
    }
}
