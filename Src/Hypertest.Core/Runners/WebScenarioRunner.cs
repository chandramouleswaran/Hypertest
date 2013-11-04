using System.Collections.Generic;
using Hypertest.Core.Interfaces;
using System;
using Hypertest.Core.Tests;
using System.Threading.Tasks;

namespace Hypertest.Core.Runners
{
    class WebScenarioRunner : IRunner
    {
        private static WebScenarioRunner _runner;

        public static WebScenarioRunner Current
        {
            get { return _runner ?? (_runner = new WebScenarioRunner()); }
        }

        private readonly Dictionary<String, Variable> _globals;
        private WebTestScenario _scenario;
        private WebScenarioRunner()
        {
            _globals = new Dictionary<string, Variable>();
        }
        public void Initialize(TestScenario scenario)
        {
            if (scenario is WebTestScenario)
            {
                _globals.Clear();
                _scenario = scenario as WebTestScenario;
            }
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Wait(int milliseconds)
        {
            throw new NotImplementedException();
        }

        public bool AddVariable(Variable variable)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public Variable GetVariable(string name)
        {
            throw new NotImplementedException();
        }

        public string UniqueID { get; private set; }
        public string RunFolder { get; private set; }
        public bool IsRunning { get; private set; }

        public TestScenario Scenario
        {
            get { return _scenario; }
        }
    }
}
