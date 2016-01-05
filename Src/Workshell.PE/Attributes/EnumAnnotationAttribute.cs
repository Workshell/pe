using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Attributes
{

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumAnnotationAttribute : Attribute
    {

        public EnumAnnotationAttribute(string name)
        {
            Name = name;
        }

        #region Properties

        public string Name
        {
            get;
            private set;
        }

        #endregion

    }

}
