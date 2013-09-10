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
using System.Collections.ObjectModel;
using System.Windows.Controls;

using Hypertest.Core;
using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.TestModel
{
    /// <summary>
    /// The looper test case that can loop for a given number of times
    /// </summary>
    [Serializable]
    [DisplayName("Loop")]
    public class LooperTestCase : TestCase
    {
        #region Member
        private static UserControl control = null;
        #endregion

        #region CTOR
        public LooperTestCase():base()
        {
            this.Name = "Looper";
            this.Expected = TestStatus.Passed;
            this.WaitTime = 0;
            this.LoopCount = 2;
        }

        public LooperTestCase(String description) : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override void Initialize(ITestRunner runner)
        {
            ObservableCollection<TestCase> folders = new ObservableCollection<TestCase>();
            //Modify the test case to use folder test case
            for (int i = 0; i < LoopCount; i++)
            {
                FolderTestCase f = new FolderTestCase("For loop " + i.ToString());
                f.Name = "For loop " + (i + 1).ToString();
                f.Children = this.Children.Clone();
                folders.Add(f);
            }
            this.Children.Clear();
            this.Children = folders;
        }

        public override TestResult Run()
        {
            result = base.Run();
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Looper);
            }
        }

        public override bool IsParent
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Property
        [PostRun]
        public int Counter
        {
            get;
            set;
        }

        public int LoopCount { get; set; }

        public new UserControl Control
        {
            get
            {
                TestEditControl c = base.Control;
                c.TestSpecificControl = new LooperControl();
                return c;
            }
        }
        #endregion

    }
}
