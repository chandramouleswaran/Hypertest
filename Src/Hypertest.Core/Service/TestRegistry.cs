using Hypertest.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Hypertest.Core.Service
{
    internal class TestRegistry : ITestRegistry
    {
        private List<Type> _types;
        
        public TestRegistry()
        {
            _types = new List<Type>();
        }

        public void Add(Type testCase)
        {
            if(!_types.Contains(testCase))
                _types.Add(testCase);
        }

        public IList<Type> Tests
        {
            get { return _types; }
        }
    }
}
