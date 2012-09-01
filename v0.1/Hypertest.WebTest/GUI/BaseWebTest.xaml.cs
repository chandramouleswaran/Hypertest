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

namespace Hypertest.WebTest
{
    /// <summary>
    /// Interaction logic for BaseWebTest.xaml
    /// </summary>
    [Serializable]
    public partial class BaseWebTest : UserControl
    {
        public BaseWebTest()
        {
            InitializeComponent();
        }


        public UserControl WebContentControl
        {
            get
            {
                ContentControl e = FindName("WebContent") as ContentControl;
                return e.Content as UserControl;
            }
            set
            {
                ContentControl e = FindName("WebContent") as ContentControl;
                e.Content = value;
            }
        }
    }
}
