﻿#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Hypertest.Core.Results;
using Hypertest.Core.Runners;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core
{
    internal class WebTestCurrentResultViewModel : ContentViewModel
    {
        private readonly WebTestResultView _resultView;

        public WebTestCurrentResultViewModel(AbstractWorkspace workspace, ICommandManager commandManager, ILoggerService logger,
            IMenuService menuService)
            : base(workspace, commandManager, logger, menuService)
        {
            _tooltip = "Web Test Results";
            _title = "Web Test Results";
            _resultView = new WebTestResultView();
        }

        /// <summary>
        /// The content ID - unique value for each document. "WEBTESTRESULT:##:" + location of the file.
        /// </summary>
        /// <value>The content id.</value>
        public override string ContentId
        {
            get { return "WEBTESTRESULT:##:" + Tooltip; }
        }

        /// <summary>
        /// Refreshes the Web test result view model
        /// </summary>
        internal void Refresh()
        {
            if (WebScenarioRunner.Current.Result != null)
            {
                WebScenarioRunner.Current.Result.Scenario.SetDirty(false);
                Model = WebScenarioRunner.Current.Result;
                View = _resultView;
                View.DataContext = Model;
            }
        }
    }
}