using System;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Interfaces
{
    public interface IRunner
    {
        void Initialize(TestScenario scenario);
        void Pause();
        void Resume();
        void Stop();
        void Wait(int milliseconds);
        bool AddVariable(Variable variable);
        void Clear();
        void CleanUp();

        Variable GetVariable(String name);
        string UniqueID { get; }
        string RunFolder { get; }
        bool IsRunning { get; }
    }
}
