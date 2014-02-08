#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Linq;
using System.Windows;
using CommandLine;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Runners;
using Hypertest.Core.Tests;
using Microsoft.Practices.Unity;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HTBootstrapper b;
        private IShell shell;
        private IOpenDocumentService documentService;
        private ILoggerService loggerService;
        private IRunner runner;
        private ParserResult<Options> cmdLineResult;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            b = new HTBootstrapper();
            b.Run();
            shell = b.Container.Resolve<IShell>();
            documentService = b.Container.Resolve<IOpenDocumentService>();
            loggerService = b.Container.Resolve<ILoggerService>();
            var window = shell as Window;
            if (window != null)
            {
                window.Loaded += OnLoaded;
                window.Unloaded += OnUnloaded;
            }
            cmdLineResult = Parser.Default.ParseArguments<Options>(e.Args);
        }

        #region Save Restore layout and use command line arguments
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            shell.LoadLayout();

            //Command line parsing - either open the file or even run the scenario based on what is opened
            runner = b.Container.Resolve<IRunner>();
            if (!cmdLineResult.Errors.Any())
            {
                if (cmdLineResult.Value.OpenFile != null)
                {
                    var cvm = documentService.Open(cmdLineResult.Value.OpenFile) as ContentViewModel;
                    var wts = (cvm != null) ? cvm.Model as WebTestScenario : null;
                    if (wts != null)
                    {
                        if (cmdLineResult.Value.Browser != null)
                        {
                            //TODO: Parse the browser and send the browser type to the initialize function
                            if (runner.IsRunning == false)
                            {
                                var scenario = wts.Clone() as TestScenario;
                                runner.Initialize(scenario);
                            }
                        }
                    }
                }
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            shell.SaveLayout();
        } 
        #endregion
    }
}