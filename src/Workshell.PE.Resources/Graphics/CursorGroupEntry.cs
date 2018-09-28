using System;
using System.Collections.Generic;
using System.Text;

using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorGroupEntry
    {
        internal CursorGroupEntry(CURSOR_RESDIR resDir)
        {
            Width = resDir.Cursor.Width;
            Height = resDir.Cursor.Height;
            Planes = resDir.Planes;
            BitCount = resDir.BitCount;
            BytesInRes = resDir.BytesInRes;
            CursorId = resDir.CursorId;
        }

        #region Methods

        public override string ToString()
        {
            return $"{Width}x{Height} {BitCount}-bit, ID: {CursorId}";
        }

        #endregion

        #region Properties

        public ushort Width { get; }
        public ushort Height { get; }
        public ushort Planes { get; }
        public ushort BitCount { get; }
        public uint BytesInRes { get; }
        public ushort CursorId { get; }

        #endregion
    }
}
