using System;

namespace JohnCena.Mset.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class PropertyOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public PropertyOrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}
