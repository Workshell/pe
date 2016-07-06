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

        private IconGroupResourceEntry[] entries;

        internal IconGroupResource(IconGroupResourceEntry[] groupEntries)
        {
            entries = groupEntries;
        }

        #region Static Methods

        public static IconGroupResource FromBytes(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                return FromStream(mem);
            }
        }

        public static IconGroupResource FromStream(Stream stream)
        {
            NEWHEADER header = Utils.Read<NEWHEADER>(stream);

            if (header.ResType != 1)
                throw new Exception("Not an icon group resource.");

            IconGroupResourceEntry[] entries = new IconGroupResourceEntry[header.ResCount];

            for(var i = 0; i < header.ResCount; i++)
            {
                ICON_RESDIR icon = Utils.Read<ICON_RESDIR>(stream);
                IconGroupResourceEntry entry = new IconGroupResourceEntry(icon);

                entries[i] = entry;
            }

            IconGroupResource group = new IconGroupResource(entries);

            return group;
        }

        public static IconGroupResource FromResource(Resource resource)
        {
            return FromResource(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static IconGroupResource FromResource(Resource resource, uint language)
        {
            byte[] data = resource.ToBytes(language);

            return FromBytes(data);
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
