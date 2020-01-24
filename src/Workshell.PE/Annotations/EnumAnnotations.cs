#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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

            #pragma warning disable CS0618 // Type or member is obsolete
            TypeSize = Marshal.SizeOf(underlayingType);
            #pragma warning restore CS0618 // Type or member is obsolete
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
