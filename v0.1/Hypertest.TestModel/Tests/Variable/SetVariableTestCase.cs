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
using System.Windows.Controls;

using Hypertest.Core;
using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.TestModel
{
    /// <summary>
    /// This test case can set a simple variable in the runner
    /// </summary>
    [Serializable]
    [DisplayName("Set Variable")]
    public class SetVariableTestCase : TestCase
    {
        #region CTOR
        public SetVariableTestCase():base()
        {
            this.Name = "Set Variable";
            this.Expected = TestStatus.None;
            this.ExitBranchOnError = true;
            this.ExitOnError = false;
        }

        public SetVariableTestCase(String description):this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override TestResult Run()
        {
            result = base.Run();

            //Gather details from the GUI
            String localValue = this.Value.Evaluate().ToString();
            Variable v = new Variable(this.Variable, localValue);

            Runner.Instance.AddVariable(v);
            
            result.Actual = TestStatus.None;
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.SetVariable);
            }
        }

        public new UserControl Control
        {
            get
            {
                TestEditControl c = base.Control;
                c.TestSpecificControl = new SetVariableControl();
                return c;
            }
        }
        #endregion

        #region Property
        public string Variable { get; set; }
        public string Value { get; set; }
        #endregion
    }
}
