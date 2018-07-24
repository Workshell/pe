using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FieldAnnotationAttribute : Attribute
    {
        public FieldAnnotationAttribute(string desc)
        {
            Description = desc;
            ArrayLength = 0;
            Flags = false;
            FlagType = null;
        }

        #region Properties

        public string Description { get; set; }
        public int ArrayLength { get; set; }
        public bool Flags { get; set; }
        public Type FlagType { get; set; }

        #endregion
    }
}
