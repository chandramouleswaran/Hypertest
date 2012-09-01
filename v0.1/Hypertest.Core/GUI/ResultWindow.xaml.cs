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
using System.Windows.Controls;

namespace Hypertest.Core.GUI
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class ResultWindow : UserControl
    {
        #region Members
        private readonly ITestRunner runner;
        private TestScenario scenario;
        #endregion

        #region CTOR
        public ResultWindow()
        {
            InitializeComponent();
            runner = Runner.Instance;
            runner.RunComplete += this.runner_RunComplete;
            runner.RefreshNeeded += this.runner_RefreshNeeded;
        }
        #endregion

        #region Methods
        public void SetScenario(TestScenario testScenario)
        {
            this.scenario = testScenario;
            runner.Clear();
            Run();
        }

        private void Run()
        {
            runner.Initialize(this.scenario);
            runner_RefreshNeeded();
            runner.Run();
        }
        #endregion

        #region Callbacks
        void runner_RefreshNeeded()
        {
            treeView1.DataContext = runner.Result;
            treeView1.Items.Refresh();
        }


        public void runner_RunComplete(object sender, EventArgs e)
        {
            runner.Stop();
        }
        #endregion
    }
}
