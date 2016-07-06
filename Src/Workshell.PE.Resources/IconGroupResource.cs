using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum IconGroupFormat
    {
        Raw,
        Resource,
        Icon
    }

    public sealed class IconGroupResourceEntry
    {

        internal IconGroupResourceEntry(ICON_RESDIR resDir)
        {
            Width = resDir.Icon.Width;
            Height = resDir.Icon.Height;
            ColorCount = resDir.Icon.ColorCount;
            Planes = resDir.Planes;
            BitCount = resDir.BitCount;
            BytesInRes = resDir.BytesInRes;
            IconId = resDir.IconId;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0}x{1} {2}-bit, ID: {3}", Width, Height, BitCount, IconId);
        }

        #endregion

        #region Properties

        public byte Width
        {
            get;
            private set;
        }

        public byte Height
        {
            get;
            private set;
        }

        public byte ColorCount
        {
            get;
            private set;
        }

        public ushort Planes
        {
            get;
            private set;
        }

        public ushort BitCount
        {
            get;
            private set;
        }

        public uint BytesInRes
        {
            get;
            private set;
        }

        public ushort IconId
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class IconGroupResource : IEnumerable<IconGroupResourceEntry>
    {

        private Resource resource;
        private uint language_id;
        private IconGroupResourceEntry[] entries;

        internal IconGroupResource(Resource sourceResource, uint languageId, IconGroupResourceEntry[] groupEntries)
        {
            resource = sourceResource;
            language_id = languageId;
            entries = groupEntries;
        }

        #region Static Methods

        public static IconGroupResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static IconGroupResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            byte[] data = resource.ToBytes(language);

            using (MemoryStream mem = new MemoryStream(data))
            {
                NEWHEADER header = Utils.Read<NEWHEADER>(mem);

                if (header.ResType != 1)
                    throw new Exception("Not an icon group resource.");

                IconGroupResourceEntry[] entries = new IconGroupResourceEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    ICON_RESDIR icon = Utils.Read<ICON_RESDIR>(mem);
                    IconGroupResourceEntry entry = new IconGroupResourceEntry(icon);

                    entries[i] = entry;
                }

                IconGroupResource group = new IconGroupResource(resource, language, entries);

                return group;
            }
        }

        #endregion

        #region Methods

        public IEnumerator<IconGroupResourceEntry> GetEnumerator()
        {
            for (var i = 0; i < entries.Length; i++)
                yield return entries[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Save(string fileName)
        {
            Save(fileName, IconGroupFormat.Icon);
        }

        public void Save(Stream stream)
        {
            Save(stream, IconGroupFormat.Icon);
        }

        public void Save(string fileName, IconGroupFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, IconGroupFormat format)
        {
            switch (format)
            {
                case IconGroupFormat.Raw:
                    SaveRaw(stream);
                    break;
                case IconGroupFormat.Resource:
                    SaveResource(stream);
                    break;
                case IconGroupFormat.Icon:
                    SaveIcon(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.ToBytes(language_id);

            stream.Write(data, 0, data.Length);
        }

        private void SaveResource(Stream stream)
        {
            //
        }

        private void SaveIcon(Stream stream)
        {
            uint[] offsets = new uint[entries.Length];
            uint offset = Convert.ToUInt32(6 + (16 * offsets.Length));

            for(var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];
                offsets[i] = offset;

                offset += entry.BytesInRes;
            }

            Utils.Write(Convert.ToUInt16(0), stream);
            Utils.Write(Convert.ToUInt16(1), stream);
            Utils.Write(Convert.ToUInt16(entries.Length), stream);

            for (var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];

                Utils.Write(Convert.ToByte(entry.Width), stream);
                Utils.Write(Convert.ToByte(entry.Height), stream);
                Utils.Write(Convert.ToByte(entry.ColorCount), stream);
                Utils.Write(Convert.ToByte(0), stream);
                Utils.Write(Convert.ToUInt16(1), stream);

                ushort colors = 0;

                if (entry.ColorCount <= 2)
                {
                    colors = 1;
                }
                else if (entry.ColorCount <= 4)
                {
                    colors = 2;
                }
                else if (entry.ColorCount <= 8)
                {
                    colors = 3;
                }
                else if (entry.ColorCount <= 16)
                {
                    colors = 4;
                }
                else if (entry.ColorCount <= 32)
                {
                    colors = 5;
                }
                else if (entry.ColorCount <= 64)
                {
                    colors = 6;
                }
                else if (entry.ColorCount <= 128)
                {
                    colors = 7;
                }
                //else if (entry.ColorCount <= 256)
                else
                {
                    colors = 0;
                }

                Utils.Write(colors, stream);
                Utils.Write(entry.BytesInRes, stream);
                Utils.Write(offsets[i], stream);
            }

            ResourceType icon_type = resource.Type.Resources.FirstOrDefault(t => t.Id == ResourceType.RT_ICON);

            for (var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];
                Resource resource = icon_type.FirstOrDefault(r => r.Id == entry.IconId);
                byte[] data = resource.ToBytes(language_id);

                Utils.Write(data, stream);                
            }
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public IconGroupResourceEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
