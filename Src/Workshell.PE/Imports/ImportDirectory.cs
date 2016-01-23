using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ImportDirectoryEntry : ILocationSupport, IRawDataSupport
    {

        private static readonly int size = Utils.SizeOf<IMAGE_IMPORT_DESCRIPTOR>();

        private ImportDirectory directory;
        private IMAGE_IMPORT_DESCRIPTOR descriptor;
        private StreamLocation location;
        private string name;

        internal ImportDirectoryEntry(ImportDirectory importDirectory, IMAGE_IMPORT_DESCRIPTOR importDescriptor, long descriptorOffset)
        {
            directory = importDirectory;
            descriptor = importDescriptor;
            location = new StreamLocation(descriptorOffset,size);
        }

        #region Methods

        public byte[] GetBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                Utils.Write<IMAGE_IMPORT_DESCRIPTOR>(descriptor,mem);

                return mem.ToArray();
            }
        }
        
        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(descriptor.TimeDateStamp);
        }

        public string GetName()
        {
            if (String.IsNullOrWhiteSpace(name))
            {           
                StringBuilder builder = new StringBuilder();
                Stream stream = directory.Content.Section.Sections.Reader.GetStream();
                long offset = Convert.ToInt64(directory.Content.Section.RVAToOffset(descriptor.Name));

                stream.Seek(offset,SeekOrigin.Begin);

                while (true)
                {
                    int b = stream.ReadByte();

                    if (b <= 0)
                        break;

                    builder.Append((char)b);
                }

                name = builder.ToString();
            }

            return name;
        }

        #endregion

        #region Static Properties

        public static int Size
        {
            get
            {
                return size;
            }
        }

        #endregion

        #region Properties

        public ImportDirectory Directory
        {
            get
            {
                return directory;
            }
        }

        public StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        [FieldAnnotation("Original First Thunk")]
        public uint OriginalFirstThunk
        {
            get
            {
                return descriptor.OriginalFirstThunk;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return descriptor.TimeDateStamp;
            }
        }

        [FieldAnnotation("Forwarder Chain")]
        public uint ForwarderChain
        {
            get
            {
                return descriptor.ForwarderChain;
            }
        }

        [FieldAnnotation("Name")]
        public uint Name
        {
            get
            {
                return descriptor.Name;
            }
        }

        [FieldAnnotation("First Thunk")]
        public uint FirstThunk
        {
            get
            {
                return descriptor.FirstThunk;
            }
        }

        #endregion

    }

    public class ImportDirectory : ILocationSupport, IRawDataSupport, IEnumerable<ImportDirectoryEntry>
    {

        private ImportContent content;
        private StreamLocation location;
        private List<ImportDirectoryEntry> entries;

        internal ImportDirectory(ImportContent importContent, IEnumerable<IMAGE_IMPORT_DESCRIPTOR> importDescriptors, StreamLocation streamLoc)
        {
            content = importContent;
            location = streamLoc;
            entries = new List<ImportDirectoryEntry>();

            long offset = streamLoc.Offset;

            foreach(IMAGE_IMPORT_DESCRIPTOR descriptor in importDescriptors)
            {
                ImportDirectoryEntry entry = new ImportDirectoryEntry(this,descriptor,offset);

                entries.Add(entry);

                offset += ImportDirectoryEntry.Size;
            }
        }

        #region Methods

        public IEnumerator<ImportDirectoryEntry> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] GetBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                foreach(ImportDirectoryEntry entry in entries)
                {
                    byte[] buffer = entry.GetBytes();

                    mem.Write(buffer,0,buffer.Length);
                }

                return mem.ToArray();
            }
        }

        #endregion

        #region Properties

        public ImportContent Content
        {
            get
            {
                return content;
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
                return entries.Count;
            }
        }

        public ImportDirectoryEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
