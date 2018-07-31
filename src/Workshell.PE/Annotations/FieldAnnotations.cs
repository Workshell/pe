﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
            var desc = Description ?? string.Empty;
            var value = (Value == null ? "(null)" : Utils.IntToHex(Value));
            var result = (Type.IsArray ? $"{desc} [{ArrayLength}]" : $"{desc} - {value}");

            return result;
        }

        #endregion

        #region Properties

        public string Description { get; }
        public int ArrayLength { get; }
        public bool Flags { get; }
        public string Name { get; }
        public Type Type { get; }
        public object Value { get; }
        public int Size { get; }
        public bool IsArray => Type.IsArray;
        public int ArraySize => (IsArray ? ArrayLength * Size : 0);

        #endregion
    }

    public sealed class FieldAnnotations : IEnumerable<FieldAnnotation>
    {
        private List<FieldAnnotation> _list;

        internal FieldAnnotations(object annotatedObject)
        {
            _list = new List<FieldAnnotation>();

            if (annotatedObject != null)
            {
                var offset = 0;

                foreach(var prop in annotatedObject.GetType().GetProperties())
                {
                    var attr = prop.GetCustomAttribute<FieldAnnotationAttribute>();

                    if (attr == null)
                        continue;

                    var desc = attr.Description;
                    var name = prop.Name;
                    var type = prop.PropertyType;
                    var size = 0;

                    #pragma warning disable CS0618 // Type or member is obsolete
                    if (type.IsArray)
                    {
                        size = Marshal.SizeOf(type.GetElementType());
                    }
                    else
                    {
                        size = Marshal.SizeOf(type);
                    }
                    #pragma warning restore CS0618 // Type or member is obsolete

                    var value = prop.GetValue(annotatedObject,null);
                    var annotation = new FieldAnnotation(desc,attr.ArrayLength,attr.Flags,name,type,value,size);

                    _list.Add(annotation);

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
            var annotations = new FieldAnnotations(annotatedObj);

            if (nullEmpty && annotations.Count == 0)
                return null;

            return annotations;
        }

        #endregion

        #region Method

        public IEnumerator<FieldAnnotation> GetEnumerator()
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
        public FieldAnnotation this[int index] => _list[index];

        #endregion
    }
}
