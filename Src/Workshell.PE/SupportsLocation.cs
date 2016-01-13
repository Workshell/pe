using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public interface ILocationSupport
    {

        #region Properties

        StreamLocation Location
        {
            get;
        }

        #endregion

    }

    public abstract class LocationSupport : ILocationSupport
    {

        #region Properties

        public abstract StreamLocation Location
        {
            get;
        }

        #endregion

    }

    public sealed class GenericLocationSupport : LocationSupport
    {

        private StreamLocation location;
        private object parent;

        public GenericLocationSupport(long offset, long size) : this(offset,size,null)
        {
        }

        public GenericLocationSupport(long offset, long size, object parentObj)
        {
            location = new StreamLocation(offset,size);
            parent = parentObj;
        }

        #region Properties

        public override StreamLocation Location
        {
            get
            {
                return location;
            }
        }

        public object Parent
        {
            get
            {
                return parent;
            }
        }

        #endregion

    }

}
