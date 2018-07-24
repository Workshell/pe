using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Workshell.PE.Annotations
{
    public sealed class EnumAnnotation<T>
    {
        internal EnumAnnotation(string headerName, Type type, T value, bool isFlags)
        {
            HeaderName = headerName;
            Type = type;
            Value = value;
            IsFlags = isFlags;
        }

        #region Properties

        public string HeaderName { get; }
        public Type Type { get; }
        public T Value { get; }
        public bool IsFlags { get; }

        #endregion
    }

    public sealed class EnumAnnotations<T> : IEnumerable<EnumAnnotation<T>>
    {
        private readonly List<EnumAnnotation<T>> _list;

        public EnumAnnotations()
        {
            _list = new List<EnumAnnotation<T>>();

            var type = typeof(T);
            
            if (!type.GetTypeInfo().IsEnum)
                throw new InvalidOperationException();

            var flagsAttr = type.GetTypeInfo().GetCustomAttribute<FlagsAttribute>();
            var fields = type.GetFields();

            foreach(var field in fields)
            {
                var attr = field.GetCustomAttribute<EnumAnnotationAttribute>();

                if (attr == null)
                    continue;

                var annotation = new EnumAnnotation<T>(attr.Name,type,(T)field.GetValue(null),flagsAttr != null);

                _list.Add(annotation);
            }

            var underlayingType = Enum.GetUnderlyingType(typeof(T));

            TypeSize = Marshal.SizeOf(underlayingType);
        }

        #region Methods

        public IEnumerator<EnumAnnotation<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count => _list.Count;
        public EnumAnnotation<T> this[int index] => _list[index];

        public EnumAnnotation<T> this[T value]
        {
            get
            {
                var annotation = _list.FirstOrDefault(a => value.Equals(a.Value));

                return annotation;
            }
        }

        public int TypeSize { get; }

        #endregion
    }
}
