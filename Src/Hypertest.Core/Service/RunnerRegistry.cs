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
using Hypertest.Core.Attributes;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;
using Microsoft.Practices.Unity;

namespace Hypertest.Core.Service
{
    internal class RunnerRegistry : IRunnerRegistry, INotifyPropertyChanged
    {
        private readonly List<Type> _types;
        private IUnityContainer _container;

        public RunnerRegistry(IUnityContainer container)
        {
            _container = container;
            _types = new List<Type>();
        }

        public IRunner this[TestScenario scenario]
        {
            get
            {
                if (scenario == null)
                    return null;

                foreach (var type in _types)
                {
                    IRunner runner = _container.Resolve(type) as IRunner;
                    if (runner != null)
                    {
                        object[] attributes = type.GetCustomAttributes(typeof(ScenarioTypesAttribute), true);
                        foreach (var attribute in attributes)
                        {
                            ScenarioTypesAttribute sta = attribute as ScenarioTypesAttribute;
                            if (sta.Type == scenario.GetType())
                                return runner;
                        }
                    }
                }
                throw new Exception("No runner found that can handle this scenario. Make sure you have the ScenarioTypes attribute for your runner class.");
            }
        }

        public void Add(Type runnerType)
        {
            if (!typeof(IRunner).IsAssignableFrom(runnerType))
            {
                throw new ArgumentException("The type you are trying to add needs to be a IRunner", "runnerType");
            }

            if (!_types.Contains(runnerType))
            {
                _types.Add(runnerType);
            }
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