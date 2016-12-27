using System;

namespace JohnCena.Mset.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class RelativeArraySizeAttribute : Attribute
    {
        public string PropertyName { get; private set; }

        public RelativeArraySizeAttribute(string prop)
        {
            this.PropertyName = prop;
        }
    }
}
