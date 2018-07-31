using System;
using System.Collections.Generic;
using System.Text;
using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class ImportDirectoryEntry : ImportDirectoryEntryBase
    {
        private readonly string _name;

        internal ImportDirectoryEntry(PortableExecutableImage image, Location location, IMAGE_IMPORT_DESCRIPTOR descriptor, string name) : base(image, location, false)
        {
            _name = name;

            OriginalFirstThunk = descriptor.OriginalFirstThunk;
            TimeDateStamp = descriptor.TimeDateStamp;
            ForwarderChain = descriptor.ForwarderChain;
            Name = descriptor.Name;
            FirstThunk = descriptor.FirstThunk;
        }

        #region Methods

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(TimeDateStamp);
        }

        public string GetName()
        {
            return _name;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Original First Thunk")]
        public uint OriginalFirstThunk { get; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; }

        [FieldAnnotation("Forwarder Chain")]
        public uint ForwarderChain { get; }

        [FieldAnnotation("Name")]
        public uint Name { get; }

        [FieldAnnotation("First Thunk")]
        public uint FirstThunk { get; }

        #endregion
    }
}
