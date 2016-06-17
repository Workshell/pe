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

        internal Export(int index, int nameIndex, uint entryPoint, string name, int ord, string forwardName)
        {
            Index = index;
            NameIndex = nameIndex;
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

        public int Index
        {
            get;
            private set;
        }

        public int NameIndex
        {
            get;
            private set;
        }

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

        public int Ordinal
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

        private class ExportItem
        {

            public int Index;
            public uint FunctionAddress;
            public int NameIndex;
            public uint NameAddress;
            public int Ordinal;

        }

        private Export[] _exports;

        internal Exports(Export[] exports)
        {
            _exports = exports;
        }

        #region Static Methods

        public static Exports Get(ExportDirectory directory, ExportTable<uint> functionAddresses, ExportTable<uint> nameAddresses, ExportTable<ushort> ordinals)
        {
            List<Export> list = new List<Export>();
            List<ExportItem> items = new List<ExportItem>();

            for (int i = 0; i < functionAddresses.Count; i++)
            {
                ExportItem item = new ExportItem()
                {
                    Index = i,
                    FunctionAddress = functionAddresses[i],
                    NameIndex = -1,
                    NameAddress = 0,
                    Ordinal = -1
                };

                items.Add(item);
            }

            for (int i = 0; i < nameAddresses.Count; i++)
            {
                ushort ord = ordinals[i];
                ExportItem item = items.FirstOrDefault(ei => ei.Index == ord);

                if (item != null)
                {
                    item.NameIndex = i;
                    item.NameAddress = nameAddresses[i];
                    item.Ordinal = (ord + directory.Base).ToInt32();
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].NameIndex == -1)
                    items[i].Ordinal = Convert.ToInt32(i + directory.Base);
            }

            Stream stream = directory.DataDirectory.Directories.Reader.GetStream();
            LocationCalculator calc = directory.DataDirectory.Directories.Reader.GetCalculator();

            foreach (ExportItem item in items)
            {
                string name = String.Empty;

                if (item.NameIndex > -1)
                {
                    Section section = calc.RVAToSection(item.NameAddress);
                    long offset = calc.RVAToOffset(section, item.NameAddress).ToInt64();

                    name = GetString(stream, offset);
                }

                string fwd_name = String.Empty;

                if (item.FunctionAddress >= directory.DataDirectory.VirtualAddress && item.FunctionAddress <= (directory.DataDirectory.VirtualAddress + directory.DataDirectory.Size))
                {
                    Section section = calc.RVAToSection(item.FunctionAddress);
                    long offset = calc.RVAToOffset(section, item.FunctionAddress).ToInt64();

                    fwd_name = GetString(stream, offset);
                }

                Export export = new Export(item.Index, item.NameIndex, item.FunctionAddress, name, item.Ordinal, fwd_name);

                list.Add(export);
            }

            Exports exports = new Exports(list.OrderBy(e => e.Ordinal).ToArray());

            return exports;
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
