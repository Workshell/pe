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
