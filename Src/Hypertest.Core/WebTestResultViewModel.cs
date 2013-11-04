using System;
using Hypertest.Core.Runners;
using Hypertest.Core.Tests;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core
{
    class WebTestResultViewModel : ContentViewModel
    {
        private WebTestResultView _resultView;

        public WebTestResultViewModel(AbstractWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService)
            : base(workspace, commandManager, logger, menuService)
        {
            _tooltip = "Web Test Results";
            _title = "Web Test Results";
            _resultView = new WebTestResultView();
        }

        /// <summary>
        /// The content ID - unique value for each document. For TextViewModels, the contentID is "FILE:##:" + location of the file.
        /// </summary>
        /// <value>The content id.</value>
        public override string ContentId
        {
            get { return "WEBTESTRESULT:##:" + Tooltip; }
        }

        /// <summary>
        /// Refreshes the Web test result view model
        /// </summary>
        public void Refresh()
        {
            this.Model = WebScenarioRunner.Current.Scenario;
            this.View = _resultView;
            this.View.DataContext = this.Model;
        }
    }
}
