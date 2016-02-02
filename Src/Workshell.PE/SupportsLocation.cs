using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public interface ISupportsLocation
    {

        #region Properties

        Location Location
        {
            get;
        }

        #endregion

    }

    public abstract class SupportsLocation : ISupportsLocation
    {

        #region Properties

        public abstract Location Location
        {
            get;
        }

        #endregion

    }

}
