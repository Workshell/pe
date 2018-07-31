using System;
using System.Collections.Generic;
using System.Text;
using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportDirectoryEntry : ImportDirectoryEntryBase
    {
        private readonly string _name;

        internal DelayedImportDirectoryEntry(PortableExecutableImage image, Location location, IMAGE_DELAY_IMPORT_DESCRIPTOR descriptor, string name) : base(image, location, true)
        {
            _name = name;

            Attributes = descriptor.Attributes;
            Name = descriptor.Name;
            ModuleHandle = descriptor.ModuleHandle;
            DelayAddressTable = descriptor.DelayAddressTable;
            DelayNameTable = descriptor.DelayNameTable;
            BoundDelayIAT = descriptor.BoundDelayIAT;
            UnloadDelayIAT = descriptor.UnloadDelayIAT;
            TimeDateStamp = descriptor.TimeDateStamp;
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

        [FieldAnnotation("Attributes")]
        public uint Attributes { get; }

        [FieldAnnotation("Name")]
        public uint Name { get; }

        [FieldAnnotation("Module Handle")]
        public uint ModuleHandle { get; }

        [FieldAnnotation("Delay Import Address Table")]
        public uint DelayAddressTable { get; }

        [FieldAnnotation("Delay Import Hint/Name Table")]
        public uint DelayNameTable { get; }

        [FieldAnnotation("Bound Delay Import Address Table")]
        public uint BoundDelayIAT { get; }

        [FieldAnnotation("Unload Delay Import Address Table")]
        public uint UnloadDelayIAT { get; }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp { get; }

        #endregion
    }
}
