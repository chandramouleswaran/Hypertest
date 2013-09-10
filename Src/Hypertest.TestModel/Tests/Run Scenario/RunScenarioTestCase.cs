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
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;

using Hypertest.Core;
using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.TestModel
{
    /// <summary>
    /// Run scenario test case - loads and runs a test case from file
    /// </summary>
    [Serializable]
    [DisplayName("Run scenario from file")]
    public class RunScenarioTestCase : TestCase
    {
        #region CTOR
        public RunScenarioTestCase() : base()
        {
            this.Name = "Run a test case";
            this.Expected = TestStatus.Passed;
            this.FilePath = "";
            this.WaitTime = 0;
        }

        public RunScenarioTestCase(String description) : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override TestResult Run()
        {
            if (File.Exists(this.FilePath))
            {
                TestScenario scenario = FileUtils.LoadFromXML(this.FilePath);
                if (scenario != null)
                {
                    this.Children = scenario.Children.Clone();
                    result = base.Run();
                }
            }
            else
            {
                result = base.Run();
                result.Description = "The file " + this.FilePath + " does not exist. Please correct the path in your test case.";
                result.Actual = TestStatus.Failed;
            }
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.RunScenario);
            }
        }
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        public new UserControl Control
        {
            get
            {
                TestEditControl c = base.Control;
                c.TestSpecificControl = new RunScenarioUI();
                return c;
            }
        }
        #endregion
    }
}

