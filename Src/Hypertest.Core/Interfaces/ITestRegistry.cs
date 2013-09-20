using System;
using System.Collections.Generic;

namespace Hypertest.Core.Interfaces
{
    public interface ITestRegistry
    {
        void Add(Type testCase);
        IList<Type> Tests { get; }
    }
}
