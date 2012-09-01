/*
    Hypertest - A web testing framework using Selenium
    Copyright (C) 2012  Chandramouleswaran Ravichandran

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    public class Runner : ITestRunner
    {
        #region Members
        private TestScenario scenario;
        private readonly Dictionary<String, Variable> globals;
        private readonly BackgroundWorker worker;
        private bool isRunning;
        public delegate void DoWorkEventHandler(object sender, EventArgs e);
        private event RunCompleteEventHandler RunComplete;
        private event RefreshUIEventHandler RefreshNeeded;
        private string uniqueID, runFolder;

        IWebDriver driver;
        //1D Array declaration
        //2D Array declaration
        //Other settings for the testScenario should be defined here!
        #endregion

        #region CTOR
        private Runner()
        {
            globals = new Dictionary<String,Variable>();
            worker = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            worker.DoWork += BackRun;
            worker.RunWorkerCompleted += this.WorkComplete;
            worker.ProgressChanged += this.Progess;
        }
        #endregion

        #region ITestRunner Members
        public void Initialize(TestScenario testScenario)
        {
            this.scenario = testScenario.Clone();
            this.scenario.Initialize(this);
            
            do
            {
                this.uniqueID = DateTime.Now.Ticks.ToString();
            }
            while (Directory.Exists(FileUtils.AppPath + Path.DirectorySeparatorChar + uniqueID));

            this.runFolder = FileUtils.AppPath + Path.DirectorySeparatorChar + uniqueID;
            Directory.CreateDirectory(runFolder);

            if (testScenario.Variables != null)
            {
                foreach (Variable v in testScenario.Variables)
                {
                    InternalAddVariable(v);
                }
            }
            InternalAddVariable(new Variable("BASEURL", testScenario.BaseURL));
        }

        public string UniqueID
        {
            get
            {
                return uniqueID;
            }
        }

        public string RunFolder
        {
            get
            {
                return runFolder;
            }
        }

        public void Refresh()
        {
            worker.ReportProgress(1);
        }

        public void Pause()
        {
            
        }

        public void Resume()
        {
            
        }

        public void Run()
        {
            if (isRunning == false)
            {
                worker.RunWorkerAsync();
                isRunning = true;
            }
            else
            {
                //TODO: Log it to the logger
            }
        }

        private void BackRun(object sender, EventArgs e)
        {
            this.scenario.Run();
        }

        private void WorkComplete(object sender, EventArgs e)
        {            
            isRunning = false;
            this.CleanUp();
            RunComplete(this, null);
        }

        private void Progess(object sender, ProgressChangedEventArgs e)
        {
            RefreshNeeded();
        }

        public void Run(string scenarioFile)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            //testScenario.Destroy(this);
            if (IsRunning)
            {
                worker.CancelAsync();
                isRunning = false;
            }
            if (driver != null)
            { 
                driver.Quit();
                driver = null;
                FileUtils.SerializeToXML(this.Result, this.runFolder + Path.DirectorySeparatorChar + "TestRun.xml");
            }
        }

        public void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public bool AddVariable(Variable variable, bool force = true)
        {
            if (IsRunning)
            {
                return InternalAddVariable(variable, force);
            }
            return false;
        }

        private bool InternalAddVariable(Variable variable, bool force = true)
        {
            Console.WriteLine("Variable: " + variable.Name + " " + variable.Value);
            if (globals.ContainsKey(variable.Name))
            {
                if (force)
                {
                    globals.Remove(variable.Name);
                }
                else
                {
                    //TODO: Log a message
                    throw new Exception("Variable already found : " + variable.Name);
                }
            }
            globals.Add(variable.Name, variable);
            return true;
        }

        public Variable GetVariable(string name)
        {
            if (globals.ContainsKey(name))
            {
                return globals[name];
            }
            return null;
        }

        public ILogger Logger
        {
            get
            {
                return null;
            }
        }

        public IWebDriver Driver
        {
            get
            {
                return driver;
            }
        }

        public TestResult Result
        {
            get
            {
                return scenario.CurrentResult;
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        public void Clear()
        {
            if (scenario != null)
            {
                scenario.ClearResults();
            }
        }

        public void CleanUp()
        {
            if (scenario != null)
            {
                scenario.CleanUp();
            }
        }

        public void Create(BrowserType type)
        {
            if (driver != null)
            {
                throw new Exception("Browser already created for this run");
            }

            switch(type)
            {
                case BrowserType.InternetExplorer:
                    driver = new InternetExplorerDriver(FileUtils.DriverPath);
                    break;
                case BrowserType.Chrome:
                    driver = new ChromeDriver(FileUtils.DriverPath);
                    break;
                case BrowserType.Firefox:
                    driver = new FirefoxDriver();
                    break;
            }
        }
        #endregion

        #region ITestRunner Events
        event RunCompleteEventHandler ITestRunner.RunComplete
        {
            add
            {
                RunComplete += value;
            }

            remove
            {
                RunComplete -= value;
            }
        }

        event RefreshUIEventHandler ITestRunner.RefreshNeeded
        {
            add
            {
                RefreshNeeded += value;
            }

            remove
            {
                RefreshNeeded -= value;
            }
        }

        #endregion

        #region Static
        public static Runner Instance = GetInstance();
        private static Runner GetInstance()
        {
            return Instance ?? (Instance = new Runner());
        }

        #endregion
    }
}
