using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE.Annotations
{

    public class EnumAnnotation<T>
    {

        internal EnumAnnotation(string headerName, Type type, T value, bool isFlags)
        {
            HeaderName = headerName;
            Type = type;
            Value = value;
            IsFlags = isFlags;
        }

        #region Properties

        public string HeaderName
        {
            get;
            private set;
        }

        public Type Type
        {
            get;
            private set;
        }

        public T Value
        {
            get;
            private set;
        }

        public bool IsFlags
        {
            get;
            private set;
        }

        #endregion

    }

    public class EnumAnnotations<T> : IEnumerable<EnumAnnotation<T>>
    {

        private List<EnumAnnotation<T>> list;

        public EnumAnnotations()
        {
            list = new List<EnumAnnotation<T>>();

            Type type = typeof(T);
            
            if (!type.IsEnum)
                throw new InvalidOperationException();

            FlagsAttribute flags_attr = type.GetCustomAttribute<FlagsAttribute>();
            FieldInfo[] fields = type.GetFields();

            foreach(FieldInfo field in fields)
            {
                EnumAnnotationAttribute attr = field.GetCustomAttribute<EnumAnnotationAttribute>();

                if (attr == null)
                    continue;

                EnumAnnotation<T> annotation = new EnumAnnotation<T>(attr.Name,type,(T)field.GetValue(null),flags_attr != null);

                list.Add(annotation);
            }
        }

        #region Methods

        public IEnumerator<EnumAnnotation<T>> GetEnumerator()
        {
            return list.GetEnumerator();
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
                return list.Count;
            }
        }

        public EnumAnnotation<T> this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public EnumAnnotation<T> this[T value]
        {
            get
            {
                EnumAnnotation<T> annotation = list.FirstOrDefault(a => value.Equals(a.Value));

                return annotation;
            }
        }

        #endregion

    }

}
