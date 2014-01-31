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
using System.ComponentModel;

namespace Hypertest.Core.Attributes
{
    //Concept from: http://www.codeproject.com/Articles/18092/Dynamic-Property-Attribute-Evaluation-at-Run-and-D
    public class DynamicBrowsableAttribute : DependsOnPropertyAttribute
    {
        public DynamicBrowsableAttribute(string property) : base(property)
        {
        }

        public DynamicBrowsableAttribute(string property, int index) : base(property, index)
        {
        }

        protected override Attribute OnEvaluateComplete(object value)
        {
            Attribute output;
            try
            {
                // check if value is provided
                if (value == null)
                    value = true; // assume default
                // create attribute
                output = new BrowsableAttribute((bool) value);
            }
            catch
            {
                output = new ReadOnlyAttribute(true);
            }
            return output;
        }
    }
}