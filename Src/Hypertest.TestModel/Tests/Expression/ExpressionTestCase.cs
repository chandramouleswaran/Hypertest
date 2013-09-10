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
using System.Xml.Serialization;

using Hypertest.Core;
using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.TestModel
{
    /// <summary>
    /// Evaluates an expression and check it against a value
    /// </summary>
    [Serializable]
    [DisplayName("Evaluate an expression")]
    public class ExpressionTestCase : TestCase
    {
        #region CTOR
        public ExpressionTestCase() : base()
        {
            this.Name = "Evaluate an expression";
            this.Expected = TestStatus.Passed;
            this.ExpectedValue = "";
            this.Expression = "";
            this.WaitTime = 0;
        }

        public ExpressionTestCase(String description) : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override TestResult Run()
        {
            result = base.Run();
            try
            {
                this.ExpressionValue = this.Expression.Evaluate();
                if (this.ExpressionValue != null && !string.IsNullOrEmpty(this.ExpectedValue))
                {
                    if (this.ExpressionValue.ToString() == this.ExpectedValue)
                    {
                        result.OutputMessage += "Expression evaluated to the expected value";
                        result.Actual = TestStatus.Passed;
                    }
                    else
                    {
                        result.Actual = TestStatus.Failed;
                        result.OutputMessage += "Expression evaluated to: " + this.ExpressionValue + "\n";
                        result.OutputMessage += "Expression expected: " + this.ExpectedValue  + "\n";
                    }
                }
                else
                {
                    if (this.ExpectedValue == "null")
                    {
                        result.Actual = TestStatus.Passed;
                    }
                    else if (string.IsNullOrEmpty(this.ExpectedValue))
                    {
                        result.Description = "Expression evaluated. No comparison performed.";
                        result.Actual = TestStatus.Passed;
                    }
                    else
                    {
                        result.Actual = TestStatus.Failed;
                        result.OutputMessage += "Expression evaluated to null";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Actual = TestStatus.Failed;
                result.OutputMessage += ex.Message;
            }
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Expression);
            }
        }
        #endregion

        #region Property
        public string Expression { get; set; }
        public string ExpectedValue { get; set; }
        [PostRun]
        [XmlIgnore]
        public object ExpressionValue { get; protected set; }

        public new UserControl Control
        {
            get
            {
                TestEditControl c = base.Control;
                c.TestSpecificControl = new ExpressionUI();
                return c;
            }
        }
        #endregion
    }
}

