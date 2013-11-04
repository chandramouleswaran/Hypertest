using System;
using System.Collections.Generic;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Interfaces
{
    public interface ITestRegistry
    {
        void Add(Type testCase);
        IEnumerable<Type> Tests { get; }
    }
}
