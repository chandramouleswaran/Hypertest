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
using System.ComponentModel;

using Hypertest.Core.GUI;
using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// Test window object that holds the test scenario details
    /// </summary>
    public class TestWindowObject : INotifyPropertyChanged
    {
        #region Members
        private string fileName;
        private readonly TestWindow control;
        private bool isSaved;
        private static int Counter = 1;
        #endregion

        #region CTOR
        public TestWindowObject(TestScenario scenario, String name = "")
        {
            if (String.IsNullOrEmpty(name))
            {
                scenario.Name = "untitled" + Counter.ToString();
                fileName = scenario.Name;
                isSaved = false;
            }
            else
            {
                fileName = name;
                isSaved = true;
            }
            Counter++;
            control = new TestWindow();
            control.SetScenario(scenario);
        }
        #endregion

        #region Methods
        public void Save(bool force = false)
        {
            if (!isSaved || force)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog { DefaultExt = ".tsc", Filter = "Test Scenario (.tsc)|*.tsc" };
                if (dlg.ShowDialog() == true)
                {
                    fileName = dlg.FileName;
                    FileUtils.SerializeToXML(control.testTree1.Current, fileName);
                    isSaved = true;
                    OnPropertyChanged("Title");
                }
            }
            else
            {
                FileUtils.SerializeToXML(control.testTree1.Current, fileName);
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Property
        public string Title
        {
            get
            {
                return control.testTree1.Current.Name;
            }
        }

        public TestWindow Content
        {
            get
            {
                return control;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
