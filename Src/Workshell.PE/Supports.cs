using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{
    public interface ISupportsLocation
    {
        #region Properties

        Location Location { get; }

        #endregion
    }

    public interface ISupportsBytes
    {
        #region Methods

        byte[] GetBytes();
        Task<byte[]> GetBytesAsync();

        #endregion
    }
}
