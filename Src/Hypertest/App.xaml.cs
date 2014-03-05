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
using System.Linq;
using System.Windows;
using CommandLine;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;
using Microsoft.Practices.Unity;
using Microsoft.Shell;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private HTBootstrapper b;
        private IShell shell;
        private IOpenDocumentService documentService;
        private ILoggerService loggerService;
        private IRunner runner;
        private ParserResult<Options> cmdLineResult;

        private const string Unique = "SOME_RANDOM_BUT_UNIQUE_STRING!!";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

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
            ParseArguments();
        }

        private void ParseArguments()
        {
            runner = b.Container.Resolve<IRunner>();
            if (!cmdLineResult.Errors.Any())
            {
                if (cmdLineResult.Value.OpenFile != null)
                {
                    var cvm = documentService.Open(cmdLineResult.Value.OpenFile);
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

        #region ISingleInstanceApp Implementation
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            args.RemoveAt(0); //Removes the executable file name
            cmdLineResult = Parser.Default.ParseArguments<Options>(args.ToArray());
            ParseArguments();
            return true;
        } 
        #endregion
    }
}