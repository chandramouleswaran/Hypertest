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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;
using Hypertest.Core.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Runners
{
    public enum BrowserType
    {
        InternetExplorer,
        Chrome,
        Firefox
    }

    public class WebScenarioRunner : IRunner
    {
        #region Members
        private readonly Dictionary<String, Variable> _globals;
        private WebTestScenario _scenario;
        #endregion

        #region CTOR
        private WebScenarioRunner()
        {
            _globals = new Dictionary<string, Variable>();
        } 
        #endregion

        #region Statics
        private static WebScenarioRunner _runner;

        public static WebScenarioRunner Current
        {
            get { return _runner ?? (_runner = new WebScenarioRunner()); }
        } 
        #endregion

        #region IRunner
        public TestScenario Scenario
        {
            get { return _scenario; }
        }

        public void Initialize(TestScenario scenario)
        {
            if (this.IsRunning == false)
            {
                this.IsRunning = true;
                _scenario = scenario as WebTestScenario;
                Task.Factory.StartNew(() =>
                                      {
                                          this.BackRun();
                                          this.WorkComplete();
                                      });

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
            return InternalAddVariable(variable);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            this._scenario = null;
            if (this.Driver != null)
            {
                this.Driver.Quit();
                this.Driver = null;
            }
            //TODO: This is where we will create a TestResult class - add Scenario inside it and add some analysis
        }

        public Variable GetVariable(string name)
        {
            if (_globals.ContainsKey(name))
            {
                return _globals[name];
            }
            return null;
        }

        public string UniqueID { get; private set; }
        public string RunFolder { get; private set; }
        public bool IsRunning { get; private set; }
        public IWebDriver Driver { get; private set; } 
        #endregion

        #region Methods
        private bool InternalAddVariable(Variable variable, bool force = true)
        {
            if (_globals.ContainsKey(variable.Name))
            {
                if (force)
                {
                    _globals.Remove(variable.Name);
                }
                else
                {
                    _scenario.LoggerService.Log("Variable " + variable.Name + "already exists. Cannot add new variable.", LogCategory.Warn, LogPriority.Low);
                }
            }
            _globals.Add(variable.Name, variable);
            return true;
        }

        private void Create(BrowserType type)
        {
            if (this.Driver != null)
            {
                throw new Exception("Browser already created for this run");
            }

            switch (type)
            {
                case BrowserType.InternetExplorer:
                    this.Driver = new InternetExplorerDriver(FileUtils.DriverPath);
                    break;
                case BrowserType.Chrome:
                    this.Driver = new ChromeDriver(FileUtils.DriverPath);
                    break;
                case BrowserType.Firefox:
                    this.Driver = new FirefoxDriver();
                    break;
            }
        }

        private void WorkComplete()
        {
            this.CleanUp();
            this.IsRunning = false;
            //TODO: Create a Result structure and store it in a file
        }

        private void BackRun()
        {
            do
            {
                this.UniqueID = DateTime.Now.Ticks.ToString();
            }
            while (Directory.Exists(FileUtils.AppPath + Path.DirectorySeparatorChar + this.UniqueID));

            this.RunFolder = FileUtils.ResultPath + Path.DirectorySeparatorChar + this.UniqueID;
            Directory.CreateDirectory(this.RunFolder);

            _globals.Clear();
            try
            {
                Create(_scenario.BrowserType);
            }
            catch (Exception)
            {

            }
            this.Driver.Navigate().GoToUrl(_scenario.URL);
            _scenario.Run();
        }
        #endregion
    }
}