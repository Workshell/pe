#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Export
    {

        internal Export(uint entryPoint, string name, uint ord, string forwardName)
        {
            EntryPoint = entryPoint;
            Name = name;
            Ordinal = ord;
            ForwardName = forwardName;
        }

        #region Methods

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(ForwardName))
            {
                return String.Format("0x{0:X8} {1:D4} {2}",EntryPoint,Ordinal,Name);
            }
            else
            {
                return String.Format("0x{0:X8} {1:D4} {2} -> {3}",EntryPoint,Ordinal,Name,ForwardName);
            }
        }

        #endregion

        #region Properties

        public uint EntryPoint
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public uint Ordinal
        {
            get;
            private set;
        }

        public string ForwardName
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class Exports : IEnumerable<Export>
    {

        private Export[] _exports;

        internal Exports(Export[] exports)
        {
            _exports = exports;
        }

        #region Static Methods

        public static Exports Get(ExecutableImage image)
        {
            ExportDirectory directory = ExportDirectory.Get(image);

            if (directory == null)
                return null;

            ExportTable<uint> function_addresses = ExportTable<uint>.GetFunctionAddressTable(directory);

            if (function_addresses == null)
                return null;

            ExportTable<uint> name_addresses = ExportTable<uint>.GetNameAddressTable(directory);

            if (name_addresses == null)
                return null;

            ExportTable<ushort> ordinals = ExportTable<short>.GetOrdinalTable(directory);

            if (ordinals == null)
                return null;

            return Get(directory, function_addresses, name_addresses, ordinals);
        }

        public static Exports Get(ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            LocationCalculator calc = directory.DataDirectory.Directories.Image.GetCalculator();
            Stream stream = directory.DataDirectory.Directories.Image.GetStream();
            List<Tuple<int, uint, uint, ushort, string, string>> name_addresses = new List<Tuple<int, uint, uint, ushort, string, string>>();

            for(var i = 0; i < nameAddresses.Count; i++)
            {
                uint name_address = nameAddresses[i];
                ushort ordinal = ordinals[i];
                uint function_address = functionAddresses[ordinal];

                long offset = calc.RVAToOffset(name_address).ToInt64();
                string name = GetString(stream, offset);
                string fwd_name = String.Empty;

                if (function_address >= directory.DataDirectory.VirtualAddress && function_address <= (directory.DataDirectory.VirtualAddress + directory.DataDirectory.Size))
                {
                    offset = calc.RVAToOffset(function_address).ToInt64();
                    fwd_name = GetString(stream, offset);
                }

                Tuple<int, uint, uint, ushort, string, string> tuple = new Tuple<int, uint, uint, ushort, string, string>(i, function_address, name_address, ordinal, name, fwd_name);

                name_addresses.Add(tuple);
            }

            List<Export> exports = new List<Export>();

            for(var i = 0; i < functionAddresses.Count; i++)
            {
                uint function_address = functionAddresses[i];
                bool is_ordinal = !name_addresses.Any(t => t.Item2 == function_address);

                if (!is_ordinal)
                {
                    Tuple<int, uint, uint, ushort, string, string> tuple = name_addresses.First(t => t.Item2 == function_address);
                    Export export = new Export(function_address, tuple.Item5, directory.Base + tuple.Item4, tuple.Item6);

                    exports.Add(export);
                }
                else
                {
                    Export export = new Export(function_address, String.Empty, Convert.ToUInt32(directory.Base + i), String.Empty);

                    exports.Add(export);
                }
            }

            Exports result = new Exports(exports.OrderBy(e => e.Ordinal).ToArray());

            return result;
        }

        private static string GetString(Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return Utils.ReadString(stream);
        }

        #endregion

        #region Methods

        public IEnumerator<Export> GetEnumerator()
        {
            for(var i = 0; i < _exports.Length; i++)
            {
                yield return _exports[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _exports.Length;
            }
        }

        public Export this[int index]
        {
            get
            {
                return _exports[index];
            }
        }

        #endregion

    }

}
