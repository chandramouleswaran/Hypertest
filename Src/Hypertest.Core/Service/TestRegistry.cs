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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Service
{
    internal class TestRegistry : ITestRegistry, INotifyPropertyChanged
    {
        private readonly List<Type> _types;

        public TestRegistry()
        {
            _types = new List<Type>();
        }

        public void Add(Type testCase)
        {
            if (!testCase.IsSubclassOf(typeof (TestCase)))
            {
                throw new ArgumentException("The type you are trying to add needs to be a TestCase", "testCase");
            }

            CategoryAttribute attributeExists =
                testCase.GetCustomAttributes(typeof (CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;

            if (attributeExists == null)
            {
                throw new ArgumentException(
                    "Your test case needs to belong to a category which needs to be listed in the Toolbox. If you do not want to see it in the Toolbox, do not add it to the registry.",
                    "testCase");
            }

            if (!_types.Contains(testCase))
            {
                _types.Add(testCase);
                RaisePropertyChanged("Tests");
            }
        }

        public IReadOnlyList<Type> Tests
        {
            get { return _types; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Should be called when a property value has changed
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}