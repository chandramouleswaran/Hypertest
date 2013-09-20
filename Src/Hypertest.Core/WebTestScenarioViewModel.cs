using Hypertest.Core.Handlers;
using Hypertest.Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core
{
    class WebTestScenarioViewModel : ContentViewModel
    {
        public WebTestScenarioViewModel(AbstractWorkspace workspace, ICommandManager commandManager, ILoggerService logger, IMenuService menuService) : base(workspace, commandManager, logger, menuService)
        {
        }

        internal void SetModel(WebTestScenario model)
        {
            this.Model = model;
        }

        internal void SetView(WebTestScenarioView view)
        {
            this.View = view;
        }

        internal void SetHandler(WebTestScenarioHandler webTestScenarioHandler)
        {
            this.Handler = webTestScenarioHandler;
        }
    }
}
