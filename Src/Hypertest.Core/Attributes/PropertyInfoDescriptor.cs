#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.ComponentModel;
using System.Reflection;

namespace Hypertest.Core.Attributes
{
    //Concept from: http://www.codeproject.com/Articles/18092/Dynamic-Property-Attribute-Evaluation-at-Run-and-D
    public class PropertyInfoDescriptor : PropertyDescriptor
    {
        private readonly PropertyInfo propInfo;

        public PropertyInfoDescriptor(PropertyInfo prop, Attribute[] attribs)
            : base(prop.Name, attribs)
        {
            propInfo = prop;
        }

        private object DefaultValue
        {
            get
            {
                if (propInfo.IsDefined(typeof (DefaultValueAttribute), false))
                {
                    return
                        ((DefaultValueAttribute) propInfo.GetCustomAttributes(typeof (DefaultValueAttribute), false)[0])
                            .Value;
                }
                return null;
            }
        }

        public override bool IsReadOnly
        {
            get { return Attributes.Contains(new ReadOnlyAttribute(true)); }
        }

        public override Type ComponentType
        {
            get { return propInfo.DeclaringType; }
        }

        public override Type PropertyType
        {
            get { return propInfo.PropertyType; }
        }

        public override object GetValue(object component)
        {
            return propInfo.GetValue(component, null);
        }

        public override bool CanResetValue(object component)
        {
            return (!IsReadOnly &
                    (DefaultValue != null && !DefaultValue.Equals(GetValue(component))));
        }

        public override void ResetValue(object component)
        {
            SetValue(component, DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            propInfo.SetValue(component, value, null);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return (!IsReadOnly &
                    (DefaultValue != null && !DefaultValue.Equals(GetValue(component))));
        }
    }
}