using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public abstract class TLSDirectory : ILocationSupport, IRawDataSupport
    {

        public static readonly int Size32 = Utils.SizeOf<IMAGE_TLS_DIRECTORY32>();
        public static readonly int Size64 = Utils.SizeOf<IMAGE_TLS_DIRECTORY64>();

        private TLSContent content;

        internal TLSDirectory(TLSContent tlsContent)
        {
            content = tlsContent;
        }

        #region Methods

        public abstract byte[] GetBytes();

        #endregion

        #region Properties

        public TLSContent Content
        {
            get
            {
                return content;
            }
        }

        public abstract StreamLocation Location
        {
            get;
        }

        [FieldAnnotation("Start Address of Raw Data")]
        public abstract ulong StartAddress
        {
            get;
        }

        [FieldAnnotation("End Address of Raw Data")]
        public abstract ulong EndAddress
        {
            get;
        }

        [FieldAnnotation("Address of Index")]
        public abstract ulong AddressOfIndex
        {
            get;
        }

        [FieldAnnotation("Address of Callbacks")]
        public abstract ulong AddressOfCallbacks
        {
            get;
        }

        [FieldAnnotation("Size of Zero Fill")]
        public abstract uint SizeOfZeroFill
        {
            get;
        }

        [FieldAnnotation("Characteristics")]
        public abstract uint Characteristics
        {
            get;
        }

        #endregion

    }

    public class TLSDirectory32 : TLSDirectory
    {

        private StreamLocation location;
        private IMAGE_TLS_DIRECTORY32 directory;

        internal TLSDirectory32(TLSContent tlsContent, long directoryOffset, IMAGE_TLS_DIRECTORY32 tlsDirectory) : base(tlsContent)
        {
            location = new StreamLocation(directoryOffset,TLSDirectory.Size32);
            directory = tlsDirectory;
        }

        #region Methods

        public override byte[] GetBytes()
        {
            byte[] buffer = new byte[TLSDirectory.Size32];

            Utils.Write<IMAGE_TLS_DIRECTORY32>(directory,buffer,0,buffer.Length);

            return buffer;
        }

        #endregion

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public override ulong StartAddress
        {
            get
            {
                return directory.StartAddress;
            }
        }

        public override ulong EndAddress
        {
            get
            {
                return directory.EndAddress;
            }
        }

        public override ulong AddressOfIndex
        {
            get
            {
                return directory.AddressOfIndex;
            }
        }

        public override ulong AddressOfCallbacks
        {
            get
            {
                return directory.AddressOfCallbacks;
            }
        }

        public override uint SizeOfZeroFill
        {
            get
            {
                return directory.SizeOfZeroFill;
            }
        }

        public override uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        #endregion

    }

    public class TLSDirectory64 : TLSDirectory
    {

        private StreamLocation location;
        private IMAGE_TLS_DIRECTORY64 directory;

        internal TLSDirectory64(TLSContent tlsContent, long directoryOffset, IMAGE_TLS_DIRECTORY64 tlsDirectory) : base(tlsContent)
        {
            location = new StreamLocation(directoryOffset,TLSDirectory.Size64);
            directory = tlsDirectory;
        }

        #region Methods

        public override byte[] GetBytes()
        {
            byte[] buffer = new byte[TLSDirectory.Size64];

            Utils.Write<IMAGE_TLS_DIRECTORY64>(directory,buffer,0,buffer.Length);

            return buffer;
        }

        #endregion

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public override ulong StartAddress
        {
            get
            {
                return directory.StartAddress;
            }
        }

        public override ulong EndAddress
        {
            get
            {
                return directory.EndAddress;
            }
        }

        public override ulong AddressOfIndex
        {
            get
            {
                return directory.AddressOfIndex;
            }
        }

        public override ulong AddressOfCallbacks
        {
            get
            {
                return directory.AddressOfCallbacks;
            }
        }

        public override uint SizeOfZeroFill
        {
            get
            {
                return directory.SizeOfZeroFill;
            }
        }

        public override uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        #endregion

    }

}
