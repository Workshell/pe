using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public class MenuItem
    {

        #region Properties

        public ushort Id
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string Shortcut
        {
            get;
            set;
        }

        public bool IsPopup
        {
            get;
            set;
        }

        #endregion

    }

    public class PopupMenuItem : MenuItem
    {

        #region Properties

        #endregion

    }

}
