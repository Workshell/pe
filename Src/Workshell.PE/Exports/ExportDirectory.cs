using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Attributes;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ExportDirectory : ILocationSupport
    {

        private static readonly int size = Utils.SizeOf<IMAGE_EXPORT_DIRECTORY>();

        private ExportContent content;
        private IMAGE_EXPORT_DIRECTORY directory;
        private StreamLocation location;

        internal ExportDirectory(ExportContent exportContent, IMAGE_EXPORT_DIRECTORY exportDir, StreamLocation streamLoc)
        {
            content = exportContent;
            directory = exportDir;
            location = streamLoc;
        }

        #region Methods

        public override string ToString()
        {
            return "Export Directory";
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[size];

            Utils.Write<IMAGE_EXPORT_DIRECTORY>(directory,buffer,0,buffer.Length);

            return buffer;
        }

        public DateTime GetTimeDateStamp()
        {
            return Utils.ConvertTimeDateStamp(directory.TimeDateStamp);
        }

        public string GetName()
        {
            StringBuilder builder = new StringBuilder();
            long offset = Convert.ToInt64(content.Section.RVAToOffset(directory.Name));
            Stream stream = content.Section.Sections.Reader.Stream;

            stream.Seek(offset,SeekOrigin.Begin);

            while (true)
            {
                int value = stream.ReadByte();

                if (value <= 0)
                    break;

                char c = (char)value;

                builder.Append(c);
            }

            return builder.ToString();
        }

        public uint[] GetFunctionAddresses()
        {
            List<uint> results = new List<uint>();
            long offset = Convert.ToInt64(content.Section.RVAToOffset(directory.AddressOfFunctions));
            Stream stream = content.Section.Sections.Reader.Stream;

            stream.Seek(offset,SeekOrigin.Begin);

            for(int i = 0; i < directory.NumberOfFunctions; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                results.Add(address);
            }

            return results.ToArray();
        }

        public uint[] GetFunctionNameAddresses()
        {
            List<uint> results = new List<uint>();
            long offset = Convert.ToInt64(content.Section.RVAToOffset(directory.AddressOfNames));
            Stream stream = content.Section.Sections.Reader.Stream;

            stream.Seek(offset,SeekOrigin.Begin);

            for(int i = 0; i < directory.NumberOfNames; i++)
            {
                uint address = Utils.ReadUInt32(stream);

                results.Add(address);
            }

            return results.ToArray();
        }

        public ushort[] GetFunctionOrdinals()
        {
            List<ushort> results = new List<ushort>();
            long offset = Convert.ToInt64(content.Section.RVAToOffset(directory.AddressOfNameOrdinals));
            Stream stream = content.Section.Sections.Reader.Stream;

            stream.Seek(offset,SeekOrigin.Begin);

            for(int i = 0; i < directory.NumberOfNames; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                results.Add(ord);
            }

            return results.ToArray();
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

        public ExportContent Content
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

        [FieldAnnotation("Characteristics")]
        public uint Characteristics
        {
            get
            {
                return directory.Characteristics;
            }
        }

        [FieldAnnotation("Date/Time Stamp")]
        public uint TimeDateStamp
        {
            get
            {
                return directory.TimeDateStamp;
            }
        }

        [FieldAnnotation("Major Version")]
        public ushort MajorVersion
        {
            get
            {
                return directory.MajorVersion;
            }
        }

        [FieldAnnotation("Minor Version")]
        public ushort MinorVersion
        {
            get
            {
                return directory.MinorVersion;
            }
        }

        [FieldAnnotation("Name")]
        public uint Name
        {
            get
            {
                return directory.Name;
            }
        }

        [FieldAnnotation("Base")]
        public uint Base
        {
            get
            {
                return directory.Base;
            }
        }

        [FieldAnnotation("Number of Functions")]
        public uint NumberOfFunctions
        {
            get
            {
                return directory.NumberOfFunctions;
            }
        }

        [FieldAnnotation("Number of Names")]
        public uint NumberOfNames
        {
            get
            {
                return directory.NumberOfNames;
            }
        }

        [FieldAnnotation("Address of Functions")]
        public uint AddressOfFunctions
        {
            get
            {
                return directory.AddressOfFunctions;
            }
        }

        [FieldAnnotation("Address of Names")]
        public uint AddressOfNames
        {
            get
            {
                return directory.AddressOfNames;
            }
        }

        [FieldAnnotation("Address of Name Ordinals")]
        public uint AddressOfNameOrdinals
        {
            get
            {
                return directory.AddressOfNameOrdinals;
            }
        }

        #endregion

    }

}
