#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Hypertest.Core.Service;
using Hypertest.Core.Toolbox;

namespace Hypertest.Core.Converters
{
    internal class TestRegistryToToolboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var testRegistry = value as TestRegistry;
            var dictionary = new SortedDictionary<string, CategoryNode>();
            if (testRegistry != null)
            {
                foreach (var testCase in testRegistry.Tests)
                {
                    var attribute =
                        testCase.GetCustomAttributes(typeof (CategoryAttribute), true).FirstOrDefault() as
                            CategoryAttribute;
                    if (!dictionary.ContainsKey(attribute.Category))
                    {
                        dictionary.Add(attribute.Category, new CategoryNode());
                        dictionary[attribute.Category].Category = attribute;
                    }
                    dictionary[attribute.Category].Nodes.Add(testCase);
                }
            }

            var nodes = new ObservableCollection<CategoryNode>();
            foreach (KeyValuePair<string, CategoryNode> kvp in dictionary)
            {
                nodes.Add(kvp.Value);
            }
            return nodes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(
                "Cannot convert from a tree to a test registry. Use converter for one way binding only.");
        }
    }
}