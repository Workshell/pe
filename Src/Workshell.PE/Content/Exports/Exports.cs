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
