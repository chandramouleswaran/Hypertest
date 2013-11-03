using System;
using System.ComponentModel;

namespace Hypertest.Core.Attributes
{
    public class DynamicReadonlyAttribute : DependsOnPropertyAttribute
    {
        public DynamicReadonlyAttribute(string property) : base(property) { }
        public DynamicReadonlyAttribute(string property, int index) : base(property, index) { }
        protected override Attribute OnEvaluateCoplete(object value)
        {
            Attribute output;
            try
            {
                // check if value is provided
                if (value == null)
                    value = false; // asume default
                // create attribute
                output = new ReadOnlyAttribute((bool)value);
            }
            catch
            {
                output = new ReadOnlyAttribute(false);
            }
            return output;
        }

    }
}