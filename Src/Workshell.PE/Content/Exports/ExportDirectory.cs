using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public class ExportDirectory : ISupportsLocation
    {

        private static readonly int size = Utils.SizeOf<IMAGE_EXPORT_DIRECTORY>();

        private ExportTableContent content;
        private Section section;
        private IMAGE_EXPORT_DIRECTORY directory;
        private Location location;

        internal ExportDirectory(ExportTableContent exportContent, Location dirLocation, Section dirSection, IMAGE_EXPORT_DIRECTORY dir)
        {
            content = exportContent;
            location = dirLocation;
            section = dirSection;
            directory = dir;
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
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            StringBuilder builder = new StringBuilder();
            long offset = Convert.ToInt64(calc.RVAToOffset(section,directory.Name));
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();

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
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            List<uint> results = new List<uint>();
            long offset = Convert.ToInt64(calc.RVAToOffset(section,directory.AddressOfFunctions));
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();

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
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            List<uint> results = new List<uint>();
            long offset = Convert.ToInt64(calc.RVAToOffset(section,directory.AddressOfNames));
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();

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
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();
            List<ushort> results = new List<ushort>();
            long offset = Convert.ToInt64(calc.RVAToOffset(section,directory.AddressOfNameOrdinals));
            Stream stream = content.DataDirectory.Directories.Reader.GetStream();

            stream.Seek(offset,SeekOrigin.Begin);

            for(int i = 0; i < directory.NumberOfNames; i++)
            {
                ushort ord = Utils.ReadUInt16(stream);

                results.Add(ord);
            }

            return results.ToArray();
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

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public Section Section
        {
            get
            {
                return section;
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
