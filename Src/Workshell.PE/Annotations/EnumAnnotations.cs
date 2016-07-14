#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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

    public sealed class EnumAnnotations<T> : IEnumerable<EnumAnnotation<T>>
    {

        private List<EnumAnnotation<T>> list;
        private int type_size;

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

            Type underlaying_type = Enum.GetUnderlyingType(typeof(T));

            type_size = Marshal.SizeOf(underlaying_type);
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

        public int TypeSize
        {
            get
            {
                return type_size;
            }
        }

        #endregion

    }

}
