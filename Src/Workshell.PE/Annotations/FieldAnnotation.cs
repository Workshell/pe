using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE.Annotations
{

    public sealed class FieldAnnotation
    {

        internal FieldAnnotation(string desc, int arrayLen, bool flags, string name, Type type, object value, int size)
        {
            Description = desc;
            ArrayLength = arrayLen;
            Flags = flags;
            Name = name;
            Type = type;
            Value = value;
            Size = size;
        }

        #region Methods

        public override string ToString()
        {
            string desc = Description ?? String.Empty;
            string value;

            if (Value == null)
            {
                value = "(null)";
            }
            else
            {
                value = Utils.IntToHex(Value);
            }

            string result;

            if (Type.IsArray)
            {
                result = String.Format("{0} [{1}]",desc,ArrayLength);
            }
            else
            {
                result = String.Format("{0} - {1}",desc,value);
            }

            return result;
        }

        #endregion

        #region Properties

        public string Description
        {
            get;
            private set;
        }

        public int ArrayLength
        {
            get;
            private set;
        }

        public bool Flags
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public object Value
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }

        public bool IsArray
        {
            get
            {
                return Type.IsArray;
            }
        }

        public int ArraySize
        {
            get
            {
                if (IsArray)
                {
                    return ArrayLength * Size;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

    }

    public sealed class FieldAnnotations : IEnumerable<FieldAnnotation>
    {

        private List<FieldAnnotation> annotations;

        internal FieldAnnotations(object annotatedObject)
        {
            annotations = new List<FieldAnnotation>();

            if (annotatedObject != null)
            {
                int offset = 0;

                foreach(PropertyInfo prop in annotatedObject.GetType().GetProperties())
                {
                    FieldAnnotationAttribute attr = prop.GetCustomAttribute<FieldAnnotationAttribute>();

                    if (attr == null)
                        continue;

                    string desc = attr.Description;
                    string name = prop.Name;
                    Type type = prop.PropertyType;
                    int size = 0;

                    if (type.IsArray)
                    {
                        size = Marshal.SizeOf(type.GetElementType());
                    }
                    else
                    {
                        size = Marshal.SizeOf(type);
                    }

                    object value = prop.GetValue(annotatedObject,null);
                    FieldAnnotation annotation = new FieldAnnotation(desc,attr.ArrayLength,attr.Flags,name,type,value,size);

                    annotations.Add(annotation);

                    offset += size;
                }
            }
        }

        #region Static Methods

        public static FieldAnnotations Get(object annotatedObj)
        {
            return Get(annotatedObj,false);
        }

        public static FieldAnnotations Get(object annotatedObj, bool nullEmpty)
        {
            FieldAnnotations annotations = new FieldAnnotations(annotatedObj);

            if (nullEmpty && annotations.Count == 0)
                return null;

            return annotations;
        }

        #endregion

        #region Method

        public IEnumerator<FieldAnnotation> GetEnumerator()
        {
            return annotations.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return annotations.Count;
            }
        }

        public FieldAnnotation this[int index]
        {
            get
            {
                return annotations[index];
            }
        }

        #endregion

    }

}
