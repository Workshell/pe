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
        public override uint Name { get; }

        [FieldAnnotation("First Thunk")]
        public uint FirstThunk { get; }

        #endregion
    }
}
