using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public class Export
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

    public class Exports : IEnumerable<Export>, IReadOnlyCollection<Export>
    {

        private ExportTableContent content;
        private List<Export> exports;

        internal Exports(ExportTableContent tableContent, List<Export> exportList)
        {
            content = tableContent;
            exports = exportList;
        }

        #region Methods

        public IEnumerator<Export> GetEnumerator()
        {
            return exports.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public ExportTableContent Content
        {
            get
            {
                return content;
            }
        }

        public int Count
        {
            get
            {
                return exports.Count;
            }
        }

        public Export this[int index]
        {
            get
            {
                return exports[index];
            }
        }

        #endregion

    }

}
