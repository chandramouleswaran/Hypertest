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
using Hypertest.Core.Events;
using Hypertest.Core.Interfaces;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Wide.Interfaces.Events;

namespace Hypertest.Report.File
{
    [Module(ModuleName = "Hypertest.Report.File")]
    [ModuleDependency("Hypertest.Core")]
    public class ReportModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        public ReportModule(IUnityContainer container, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashMessageUpdateEvent>().Publish(new SplashMessageUpdateEvent {Message = "Loading Report Module"});
            _eventAggregator.GetEvent<RunStartedEvent>().Subscribe(Started, ThreadOption.BackgroundThread, true);
            _eventAggregator.GetEvent<RunEndedEvent>().Subscribe(Ended, ThreadOption.BackgroundThread, true);
        }

        private void Ended(IRunner runner)
        {
            ITestRegistry registry = _container.Resolve<ITestRegistry>();
            FileReport report = new FileReport(runner, registry);
            report.Process();
        }

        private void Started(IRunner runner)
        {
            //TODO
        }
    }
}