using System;
using System.ComponentModel;

namespace Hypertest.Core.Attributes
{
    public class DynamicBrowsableAttribute : DependsOnPropertyAttribute
    {
        public DynamicBrowsableAttribute(string property) : base(property) { }
        public DynamicBrowsableAttribute(string property, int index) : base(property, index) { }
        protected override Attribute OnEvaluateCoplete(object value)
        {
            Attribute output;
            try
            {
                // check if value is provided
                if (value == null)
                    value = true; // asume default
                // create attribute
                output = new BrowsableAttribute((bool)value);
            }
            catch
            {
                output = new ReadOnlyAttribute(true);
            }
            return output;
        }


    }
}