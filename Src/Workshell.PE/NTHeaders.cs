using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class NTHeaders : ILocatable
    {

        public const uint PE_MAGIC_MZ = 17744;

        private ExeReader reader;
        private StreamLocation location;
        private FileHeader file_header;
        private OptionalHeader opt_header;

        internal NTHeaders(ExeReader exeReader, StreamLocation streamLoc, FileHeader fileHeader, OptionalHeader optHeader)
        {
            reader = exeReader;
            location = streamLoc;
            file_header = fileHeader;
            opt_header = optHeader;
        }

        #region Methods

        public override string ToString()
        {
            return "NT Headers";
        }

        #endregion

        #region Properties

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public FileHeader FileHeader
        {
            get
            {
                return file_header;
            }
        }

        public OptionalHeader OptionalHeader
        {
            get
            {
                return opt_header;
            }
        }

        #endregion

    }

}
