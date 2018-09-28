#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
        public override uint Name { get; }

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
