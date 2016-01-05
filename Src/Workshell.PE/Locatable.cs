using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public interface ILocatable
    {

        #region Properties

        StreamLocation Location
        {
            get;
        }

        #endregion

    }

    public abstract class Locatable : ILocatable
    {

        #region Properties

        public abstract StreamLocation Location
        {
            get;
        }

        #endregion

    }

    public sealed class GenericLocatable : Locatable
    {

        private StreamLocation location;

        public GenericLocatable(long offset, long size)
        {
            location = new StreamLocation(offset,size);
        }

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        #endregion

    }

}
