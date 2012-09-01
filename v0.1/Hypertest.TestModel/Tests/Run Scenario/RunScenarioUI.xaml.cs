﻿/*
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
using System.Windows.Controls;

namespace Hypertest.TestModel
{
    /// <summary>
    /// Interaction logic for RunScenarioUI.xaml
    /// </summary>
    public partial class RunScenarioUI : UserControl
    {
        public RunScenarioUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                { DefaultExt = ".tsc", Filter = "Test Scenario (.tsc)|*.tsc" };
            if (dlg.ShowDialog() == true)
            {
                TextBox box = FindName("txtFilePath") as TextBox;
                if (box != null)
                {
                    box.Text = dlg.FileName;
                    box.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            }
        }
    }
}
