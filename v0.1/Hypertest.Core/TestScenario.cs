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
using System.Collections.ObjectModel;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    [TestIgnore]
    public class TestScenario : TestCase
    {
        #region CTOR
        public TestScenario()
        {
            this.BaseURL = "http://www.google.com";
            this.BrowserType = BrowserType.Chrome;
            this.Variables = new ObservableCollection<Variable>();
        }
        #endregion
        
        #region Property
        public override bool IsParent
        {
            get
            {
                return true;
            }
        }

        public ScenarioType ScenarioType { get; set; }

        public BrowserType BrowserType { get; set; }

        public String BaseURL { get; set; }

        public ObservableCollection<Variable> Variables { get; set; }

        #endregion

        #region Methods
        public override void Initialize(ITestRunner runner)
        {
            this.Description = "TestScenario";
            if (this.ScenarioType == ScenarioType.Web)
            {
                runner.Create(this.BrowserType);
                runner.Driver.Navigate().GoToUrl(this.BaseURL);
            }
        }

        #endregion
    }
}
