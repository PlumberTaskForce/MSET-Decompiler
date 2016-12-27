using System.Reflection;

namespace JohnCena.Mset.Data.Attributes
{
    internal class PropertyAttributes
    {
        public PropertyInfo Property { get; set; }
        public object Instance { get; set; }
        public object Value { get; set; }
        public DataType Type { get; set; }
        public int DataSize { get; set; }

        public int Order { get; set; }
        public bool Ignore { get; set; }
        public bool IsBigEndian { get; set; }
        public int ArraySize { get; set; }
        public bool IsArraySizeRelative { get; set; }

        public string RelatedPropertyName { get; set; }
        public PropertyAttributes RelatedProperty { get; set; }
        public MethodInfo CalculateMethod { get; set; }

        public bool IsResolved { get; set; }

        public void Set(object value)
        {
            this.Property.SetValue(this.Instance, value);
            this.Value = value;
            this.IsResolved = true;
        }

        public void SetUnresolved(object value)
        {
            this.Property.SetValue(this.Instance, value);
            this.Value = value;
        }
    }
}
