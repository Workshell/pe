using System;
using System.Collections.Generic;
using System.Text;

using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class IconGroupEntry
    {
        internal IconGroupEntry(ICON_RESDIR resDir)
        {
            Width = resDir.Icon.Width;
            Height = resDir.Icon.Height;
            ColorCount = resDir.Icon.ColorCount;
            Planes = resDir.Planes;
            BitCount = resDir.BitCount;
            BytesInRes = resDir.BytesInRes;
            IconId = resDir.IconId;

            if (Width == 0)
                Width = 256;

            if (Height == 0)
                Height = 256;
        }

        #region Methods

        public override string ToString()
        {
            return $"{Width}x{Height} {BitCount}-bit, ID: {IconId}";
        }

        #endregion

        #region Properties

        public ushort Width { get; }
        public ushort Height { get; }
        public byte ColorCount { get; }
        public ushort Planes { get; }
        public ushort BitCount { get; }
        public uint BytesInRes { get; }
        public ushort IconId { get; }

        #endregion
    }
}
