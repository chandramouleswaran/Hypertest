using System;
using System.ComponentModel;
using System.Reflection;

namespace Hypertest.Core.Attributes
{
    public class PropertyInfoDescriptor : PropertyDescriptor
    {

        private PropertyInfo propInfo;

        public PropertyInfoDescriptor(PropertyInfo prop, Attribute[] attribs)
            : base(prop.Name, attribs)
        {
            propInfo = prop;
        }
        private object DefaultValue
        {
            get
            {
                if (propInfo.IsDefined(typeof(DefaultValueAttribute), false))
                {
                    return ((DefaultValueAttribute)propInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false)[0]).Value;
                }
                return null;
            }
        }
        public override bool IsReadOnly
        {
            get { return this.Attributes.Contains(new System.ComponentModel.ReadOnlyAttribute(true)); }
        }

        public override object GetValue(object component)
        {
            return propInfo.GetValue(component, null);
        }

        public override bool CanResetValue(object component)
        {
            return (!this.IsReadOnly & (this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component))));
        }

        public override void ResetValue(object component)
        {
            this.SetValue(component, this.DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            propInfo.SetValue(component, value, null);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return (!this.IsReadOnly & (this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component))));
        }

        public override Type ComponentType
        {
            get
            {
                return propInfo.DeclaringType;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return propInfo.PropertyType;
            }
        }
    }
}
