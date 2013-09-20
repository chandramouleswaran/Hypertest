using System;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Interfaces
{
    interface IRunnerRegistry
    {
        void Add(IRunner runner, Type t);
        IRunner this[TestScenario scenario] { get; }
    }
}
