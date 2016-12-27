using System;

namespace JohnCena.Mset.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class CalculateSizeAttribute : Attribute
    {
        public string MethodName { get; private set; }
        public string ParameterProperty { get; private set; }

        public CalculateSizeAttribute(string method, string param_prop)
        {
            this.MethodName = method;
            this.ParameterProperty = param_prop;
        }
    }
}
