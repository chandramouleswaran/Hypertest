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

using System.Windows;

using Hypertest.Core.Utils;

namespace Hypertest.Core.GUI
{
    /// <summary>
    /// Interaction logic for ScenarioSettings.xaml
    /// </summary>
    public partial class ScenarioSettings : Window
    {
        TestScenario scenario;

        public ScenarioSettings()
        {
            InitializeComponent();
        }

        public void InitializeForScenario(TestScenario test)
        {
            scenario = test.Clone();
            this.DataContext = scenario;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(scenario.Name))
            {
                MessageBox.Show("Please enter a name for the scenario.", "Scenario name required");
                e.Handled = true;
                return;
            }

            if (string.IsNullOrEmpty(scenario.Description))
            {
                MessageBox.Show("Please enter a description for the scenario.", "Scenario description required");
                e.Handled = true;
                return;
            }

            e.Handled = true;
            this.DialogResult = true;
            this.Close();
        }

        public TestScenario NewScenario
        {
            get
            {
                return scenario;
            }
        }

    }
}
