using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public abstract class TLSDirectory : ISupportsLocation, ISupportsBytes
    {

        private TLSTableContent content;
        private Location location;

        internal TLSDirectory(TLSTableContent tlsContent, Location tlsLocation)
        {
            content = tlsContent;
            location = tlsLocation;
        }

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();
            byte[] buffer = Utils.ReadBytes(stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public TLSTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
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

    public sealed class TLSDirectory32 : TLSDirectory
    {

        private IMAGE_TLS_DIRECTORY32 directory;

        internal TLSDirectory32(TLSTableContent tlsContent, Location tlsLocation, IMAGE_TLS_DIRECTORY32 tlsDirectory) : base(tlsContent,tlsLocation)
        {
            directory = tlsDirectory;
        }

        #region Properties

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

        private IMAGE_TLS_DIRECTORY64 directory;

        internal TLSDirectory64(TLSTableContent tlsContent, Location tlsLocation, IMAGE_TLS_DIRECTORY64 tlsDirectory) : base(tlsContent,tlsLocation)
        {
            directory = tlsDirectory;
        }

        #region Properties

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
