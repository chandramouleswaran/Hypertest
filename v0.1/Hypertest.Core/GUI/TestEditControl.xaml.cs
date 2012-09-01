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

namespace Hypertest.Core.GUI
{
    /// <summary>
    /// Interaction logic for TestEditControl.xaml
    /// </summary>
    public partial class TestEditControl : UserControl
    {
        public TestEditControl()
        {
            InitializeComponent();
        }

        public UserControl TestSpecificControl 
        { 
            get
            {
                Expander e = FindName("TestSpecificContent") as Expander;
                if (e != null)
                {
                    return e.Content as UserControl;
                }
                return null;
            }
            set
            {
                Expander e = FindName("TestSpecificContent") as Expander;
                if (e != null)
                {
                    e.Content = value;
                }
            }
        }

        /// <summary>
        /// Handles the DataContextChanged event of the Variables control. This basically sets the combo boxes souce to the properties of the class.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Variables_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TestCase c = (e.NewValue as TestCase);
            if (c != null)
            {
                Variables.Columns[1].SetValue(DataGridComboBoxColumn.ItemsSourceProperty, c.PostRunPropsString);
            }
        }
    }
}
