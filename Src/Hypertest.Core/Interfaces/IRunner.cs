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
using Hypertest.Core.Results;
using Hypertest.Core.Tests;
using OpenQA.Selenium;

namespace Hypertest.Core.Interfaces
{
    public interface IRunner
    {
        string UniqueID { get; }
        string RunFolder { get; }
        bool IsRunning { get; }
        IWebDriver Driver { get; }
        TestResultModel Result { get; }

        void Initialize(TestScenario scenario);
        void Pause();
        void Resume();
        void Stop();
        void Wait(int milliseconds);
        bool AddVariable(Variable variable);
        void Clear();
        void CleanUp();
        string PrintDebug();

        Variable GetVariable(String name);
    }
}