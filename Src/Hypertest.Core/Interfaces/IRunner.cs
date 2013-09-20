using System;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Interfaces
{
    interface IRunner
    {
        void Initialize(TestScenario testScenario);
        void Pause();
        void Resume();
        void Run();
        void Refresh();
        void Run(string scenarioFile);
        void Stop();
        void Wait(int milliseconds);
        bool AddVariable(Variable variable, bool force = true);
        void Clear();
        void CleanUp();

        Variable GetVariable(String name);
        string UniqueID { get; }
        string RunFolder { get; }
        bool IsRunning { get; }
    }
}
