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
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Hypertest.Core.Attributes
{
    public class DynamicTypeDescriptor
    {
        public static PropertyDescriptorCollection GetProperties(object component)
        {
            PropertyInfo[] propsInfo = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var list = new ArrayList(propsInfo.Length);

            foreach (PropertyInfo prop in propsInfo)
            {
                var attributeList = new ArrayList();
                foreach (Attribute attrib in prop.GetCustomAttributes(true))
                {
                    if (attrib is DependsOnPropertyAttribute)
                        attributeList.Add(((DependsOnPropertyAttribute) attrib).Evaluate(component));
                    else
                        attributeList.Add(attrib);
                }
                list.Add(new PropertyInfoDescriptor(prop, (Attribute[]) attributeList.ToArray(typeof (Attribute))));
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[]) list.ToArray(typeof (PropertyDescriptor)));
        }
    }
}