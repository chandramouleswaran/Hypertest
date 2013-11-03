using System;
using System.Reflection;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Attributes
{
    /// <summary>
    /// Base class for all dynamic attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of class
        /// </summary>
        /// <param name="expression">Property name</param>
        protected DependsOnPropertyAttribute(string property)
            : base()
        {
            _property = property;
            _index = null;
        }
        /// <summary>
        /// Create new instance of class
        /// </summary>
        /// <param name="property">Property name</param>
        /// <param name="index">Property element index</param>
        protected DependsOnPropertyAttribute(string property, int index)
        {
            _property = property;
            _index = new object[] { index };
        }

        private string _property;
        private object[] _index;

        /// <summary>
        /// Evaluate attribute using property container supplied
        /// </summary>
        /// <param name="container">Object that contains property to evaluate</param>
        /// <returns>Dynamically evaluated attribute</returns>
        public Attribute Evaluate(object container)
        {
            return OnEvaluateCoplete(RuntimeEvaluator.Eval(container, _property, _index));
        }
        /// <summary>
        /// Specific dynamic attribute check implementation
        /// </summary>
        /// <param name="value">Evaluated value</param>
        /// <returns>Dynamically evaluated attribute</returns>
        protected abstract Attribute OnEvaluateCoplete(object value);

        private class RuntimeEvaluator
        {
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
                        return ((TestRunState)pInfo.GetValue(container, index) != TestRunState.NotStarted);
                    }
                }

                return null;
            }
        }
    }
}
