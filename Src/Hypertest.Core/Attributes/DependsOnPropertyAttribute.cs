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
using System.Reflection;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Attributes
{
    /// <summary>
    ///     Base class for all dynamic attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class DependsOnPropertyAttribute : Attribute
    {
        private readonly object[] _index;
        private readonly string _property;

        /// <summary>
        ///     Create new instance of class
        /// </summary>
        /// <param name="expression">Property name</param>
        protected DependsOnPropertyAttribute(string property)
        {
            _property = property;
            _index = null;
        }

        /// <summary>
        ///     Create new instance of class
        /// </summary>
        /// <param name="property">Property name</param>
        /// <param name="index">Property element index</param>
        protected DependsOnPropertyAttribute(string property, int index)
        {
            _property = property;
            _index = new object[] {index};
        }

        /// <summary>
        ///     Evaluate attribute using property container supplied
        /// </summary>
        /// <param name="container">Object that contains property to evaluate</param>
        /// <returns>Dynamically evaluated attribute</returns>
        public Attribute Evaluate(object container)
        {
            return OnEvaluateCoplete(RuntimeEvaluator.Eval(container, _property, _index));
        }

        /// <summary>
        ///     Specific dynamic attribute check implementation
        /// </summary>
        /// <param name="value">Evaluated value</param>
        /// <returns>Dynamically evaluated attribute</returns>
        protected abstract Attribute OnEvaluateCoplete(object value);

        private class RuntimeEvaluator
        {
            //Concept from: http://www.codeproject.com/Articles/18092/Dynamic-Property-Attribute-Evaluation-at-Run-and-D
            public static object Eval(object container, string property, object[] index)
            {
                PropertyInfo pInfo = container.GetType().GetProperty(property);
                if (pInfo != null)
                {
                    if (pInfo.PropertyType == typeof (bool))
                    {
                        return pInfo.GetValue(container, index);
                    }

                    if (pInfo.PropertyType == typeof (TestRunState))
                    {
                        return ((TestRunState) pInfo.GetValue(container, index) != TestRunState.NotStarted);
                    }
                }

                return null;
            }
        }
    }
}