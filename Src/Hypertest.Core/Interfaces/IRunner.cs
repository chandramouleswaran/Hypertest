using System;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Interfaces
{
    public interface IRunner
    {
        void Initialize();
        void Pause();
        void Resume();
        void Stop();
        void Wait(int milliseconds);
        bool AddVariable(Variable variable);
        void Clear();
        void CleanUp();
        void Enqueue(TestScenario scenario);
        void Dequeue(TestScenario scenario);

        Variable GetVariable(String name);
        string UniqueID { get; }
        string RunFolder { get; }
        bool IsRunning { get; }
    }
}
