using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class ImportLookupTable : ILocationSupport, IRawDataSupport, IEnumerable<ImportLookupTableEntry>
    {

        private ImportLookupTables tables;
        private ImportDirectoryEntry entry;
        private List<ImportLookupTableEntry> list;
        private StreamLocation location;

        internal ImportLookupTable(ImportLookupTables lookupTables, ImportDirectoryEntry directoryEntry, Stream stream)
        {
            tables = lookupTables;
            entry = directoryEntry;
            list = new List<ImportLookupTableEntry>();

            long ilt_offset = 0;

            if (entry.OriginalFirstThunk != 0)
            {
                ilt_offset = Convert.ToInt64(lookupTables.Content.Section.RVAToOffset(entry.OriginalFirstThunk));
            }
            else
            {
                ilt_offset = Convert.ToInt64(lookupTables.Content.Section.RVAToOffset(entry.FirstThunk));
            }

            long ilt_size = 0;
            long entry_offset = ilt_offset;

            stream.Seek(ilt_offset,SeekOrigin.Begin);

            while (true)
            {
                if (lookupTables.Content.Section.Sections.Reader.Is32Bit)
                {
                    byte[] buffer = new byte[4];

                    stream.Read(buffer,0,buffer.Length);

                    ilt_size += 4;
                    entry_offset += 4;

                    uint value = BitConverter.ToUInt32(buffer,0);

                    if (value == 0)
                        break;

                    list.Add(new ImportLookupTableEntry32(this,entry_offset,value));
                }
                else
                {
                    byte[] buffer = new byte[8];

                    stream.Read(buffer,0,buffer.Length);

                    ilt_size += 8;
                    entry_offset += 8;

                    ulong value = BitConverter.ToUInt64(buffer,0);

                    if (value == 0)
                        break;

                    list.Add(new ImportLookupTableEntry64(this,entry_offset,value));
                }
            }

            location = new StreamLocation(ilt_offset,ilt_size);
        }

        #region Methods

        public IEnumerator<ImportLookupTableEntry> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            byte[] buffer = Utils.ReadBytes(tables.Content.Section.Sections.Reader.Stream,location);

            return buffer;
        }

        #endregion

        #region Properties

        public ImportLookupTables Tables
        {
            get
            {
                return tables;
            }
        }

        public ImportDirectoryEntry DirectoryEntry
        {
            get
            {
                return entry;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public ImportLookupTableEntry this[int index]
        {
            get
            {
                return list[index];
            }
        }

        #endregion

    }

}
