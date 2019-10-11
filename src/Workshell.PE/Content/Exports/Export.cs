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

namespace Workshell.PE.Content
{
    public sealed class Export
    {
        internal Export(uint entryPoint, string name, uint ord, string forwardName)
        {
            EntryPoint = entryPoint;
            Name = name ?? string.Empty;
            Ordinal = ord;
            ForwardName = forwardName ?? string.Empty;
        }

        #region Methods

        public override string ToString()
        {
            string result;

            if (string.IsNullOrWhiteSpace(ForwardName))
            {
                result = $"0x{EntryPoint:X8} {Ordinal:D4} {Name}";
            }
            else
            {
                result = $"0x{EntryPoint:X8} {Ordinal:D4} {Name} -> {ForwardName}";
            }

            return result;
        }

        #endregion

        #region Properties

        public uint EntryPoint { get; }
        public string Name { get; }
        public uint Ordinal { get; }
        public string ForwardName { get; }

        #endregion
    }
}
