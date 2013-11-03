using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Hypertest.Core.Attributes
{
    public class DynamicTypeDescriptor
    {
        public DynamicTypeDescriptor() { }

        public static PropertyDescriptorCollection GetProperties(object component)
        {
            PropertyInfo[] propsInfo = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            ArrayList list = new ArrayList(propsInfo.Length);

            foreach (PropertyInfo prop in propsInfo)
            {
                ArrayList attributeList = new ArrayList();
                foreach (Attribute attrib in prop.GetCustomAttributes(true))
                {
                    if (attrib is DependsOnPropertyAttribute)
                        attributeList.Add(((DependsOnPropertyAttribute)attrib).Evaluate(component));
                    else
                        attributeList.Add(attrib);
                }
                list.Add(new PropertyInfoDescriptor(prop, (Attribute[])attributeList.ToArray(typeof(Attribute))));
            }
            return new PropertyDescriptorCollection((PropertyDescriptor[])list.ToArray(typeof(PropertyDescriptor)));
        }
    }
}
