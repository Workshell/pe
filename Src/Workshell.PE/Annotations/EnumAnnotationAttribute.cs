using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Annotations
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumAnnotationAttribute : Attribute
    {
        public EnumAnnotationAttribute(string name)
        {
            Name = name;
        }

        #region Properties

        public string Name { get; }
        public string Description { get; set; }

        #endregion
    }
}
