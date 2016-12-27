using System;
using System.IO;
using System.Text;

namespace JohnCena.Mset.IO
{
    internal class FormattedStreamWriter : StreamWriter
    {
        public override IFormatProvider FormatProvider { get { return this.fmtprovider; } }

        private IFormatProvider fmtprovider;

        public FormattedStreamWriter(Stream stream, Encoding encoding, int buffer_size, bool leave_open, IFormatProvider format_provider)
            : base(stream, encoding, buffer_size, leave_open)
        {
            this.fmtprovider = format_provider;
        }
    }
}
