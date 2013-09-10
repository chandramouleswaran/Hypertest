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

using Hypertest.Core;
using Hypertest.Core.Utils;

namespace Hypertest.TestModel
{
    /// <summary>
    /// The folder test case returns the cumulative result of all the test cases in the folder
    /// </summary>
    [Serializable]
    [DisplayName("Folder")]
    public class FolderTestCase : TestCase
    {
        #region CTOR
        public FolderTestCase():base()
        {
            this.Name = "Folder";
            this.Expected = TestStatus.Passed;
            this.WaitTime = 0;
        }
        
        public FolderTestCase(String description) : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides

        public override TestResult Run()
        {
            result = base.Run();
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Folder);
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

    }
}

