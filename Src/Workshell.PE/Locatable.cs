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
        private object parent;

        public GenericLocatable(long offset, long size) : this(offset,size,null)
        {
        }

        public GenericLocatable(long offset, long size, object parentObj)
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
