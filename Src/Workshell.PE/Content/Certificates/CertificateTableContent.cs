using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class CertificateTableContent : DataDirectoryContent
    {

        private Certificate cert;

        internal CertificateTableContent(DataDirectory dataDirectory, ulong imageBase) : base(dataDirectory, imageBase)
        {
            LocationCalculator calc = dataDirectory.Directories.Reader.GetCalculator();
            Stream stream = dataDirectory.Directories.Reader.GetStream();

            Load(calc, stream, imageBase);
        }

        #region Methods

        private void Load(LocationCalculator calc, Stream stream, ulong imageBase)
        {
            ulong offset = DataDirectory.VirtualAddress;
            Location location = new Location(offset, DataDirectory.VirtualAddress, imageBase + offset, DataDirectory.Size, DataDirectory.Size);

            stream.Seek(Convert.ToInt64(offset),SeekOrigin.Begin);

            WIN_CERTIFICATE win_cert = Utils.Read<WIN_CERTIFICATE>(stream);

            cert = new Certificate(this,location,win_cert);
        }

        #endregion

        #region Properties

        public Certificate Certificate
        {
            get
            {
                return cert;
            }
        }

        #endregion

    }

}
