#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

namespace Hypertest.Core.Manager
{
    public class CommonPropertyChange : Change
    {
        public CommonPropertyChange(object obj1, object obj2, string property)
        {
            Object1 = obj1;
            Object2 = obj2;
            this.property = property;
        }

        public object Object1 { get; internal set; }

        public object Object2 { get; internal set; }

        public string property { get; internal set; }

        internal override void Undo()
        {
            object backup = Object1.GetType().GetProperty(property).GetValue(Object1);
            Object1.GetType()
                .GetProperty(property)
                .SetValue(Object1, Object2.GetType().GetProperty(property).GetValue(Object2), null);
            Object2.GetType().GetProperty(property).SetValue(Object2, backup, null);
        }

        internal override void Redo()
        {
            object backup = Object1.GetType().GetProperty(property).GetValue(Object1);
            Object1.GetType()
                .GetProperty(property)
                .SetValue(Object1, Object2.GetType().GetProperty(property).GetValue(Object2), null);
            Object2.GetType().GetProperty(property).SetValue(Object2, backup, null);
        }
    }
}