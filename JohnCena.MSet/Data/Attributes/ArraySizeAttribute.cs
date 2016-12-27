using System;

namespace JohnCena.Mset.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class ArraySizeAttribute : Attribute
    {
        public int ArraySize { get; private set; }

        public ArraySizeAttribute(int size)
        {
            this.ArraySize = size;
        }
    }
}
