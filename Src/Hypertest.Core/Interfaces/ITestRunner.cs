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

using OpenQA.Selenium;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// Interface to run tests
    /// </summary>
    public delegate void RunCompleteEventHandler(object sender, EventArgs e);
    public delegate void RefreshUIEventHandler();

    public interface ITestRunner
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
        void Create(BrowserType type);
        event RunCompleteEventHandler RunComplete;
        event RefreshUIEventHandler RefreshNeeded;

        Variable GetVariable(String name);
        ILogger Logger { get; }
        TestResult Result { get; }
        IWebDriver Driver { get; }
        string UniqueID { get; }
        string RunFolder { get; }
        bool IsRunning { get; }
    }
}
