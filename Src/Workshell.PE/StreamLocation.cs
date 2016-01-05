using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class StreamLocation : IEquatable<StreamLocation>
    {

        public StreamLocation(int offset, int size)
        {
            Offset = offset;
            Size = size;
        }

        public StreamLocation(long offset, long size)
        {
            Offset = offset;
            Size = size;
        }

        public StreamLocation(uint offset, uint size)
        {
            Offset = offset;
            Size = size;
        }

        public StreamLocation(ulong offset, ulong size)
        {
            Offset = Convert.ToInt64(offset);
            Size = Convert.ToInt64(size);
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("0x{0}+{1}",Offset.ToString("X8"),Size);
        }

        public override int GetHashCode()
        {
            int prime = 397;
            int result = 0;

            result = (result * prime) ^ Offset.GetHashCode();
            result = (result * prime) ^ Size.GetHashCode();

            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StreamLocation);
        }

        public bool Equals(StreamLocation other)
        {
            if (other == null)
                return false;

            if (other == this)
                return true;

            if (Offset != other.Offset)
                return false;

            if (Size != other.Size)
                return false;

            return true;
        }

        #endregion

        #region Properties

        public long Offset
        {
            get;
            private set;
        }

        public long Size
        {
            get;
            private set;
        }

        #endregion

    }

}
