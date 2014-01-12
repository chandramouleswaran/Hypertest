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
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Runners
{
    internal class WebScenarioRunner : IRunner
    {
        private static WebScenarioRunner _runner;

        private readonly Dictionary<String, Variable> _globals;
        private WebTestScenario _scenario;

        private WebScenarioRunner()
        {
            _globals = new Dictionary<string, Variable>();
        }

        public static WebScenarioRunner Current
        {
            get { return _runner ?? (_runner = new WebScenarioRunner()); }
        }

        public TestScenario Scenario
        {
            get { return _scenario; }
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
    }
}