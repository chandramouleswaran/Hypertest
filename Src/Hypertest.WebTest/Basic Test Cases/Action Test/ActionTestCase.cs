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

using OpenQA.Selenium;

using Hypertest.Core;
using Hypertest.Core.Utils;

namespace Hypertest.WebTest
{
    /// <summary>
    /// Takes action on a given web element
    /// </summary>
    [Serializable]
    [DisplayName("Take Action on Element")]
    public class ActionTestCase : WebTestCase
    {
        #region Property
        public ElementAction Action { get; set; }
        public string Keys { get; set; }
        #endregion

        #region CTOR
        public ActionTestCase() : base()
        {
            this.Name = "Take action on the element";
            this.Expected = TestStatus.Passed;
            this.WaitTime = 2000;
            this.Action = ElementAction.None;
            this.ShowBaseContent = true;
        }

        public ActionTestCase(String description) : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override TestResult Run()
        {
            IWebDriver driver = Runner.Instance.Driver;
            result = base.Run();
            try
            {
                element = this.GetWebElement(driver);
                if (element == null || element.InnerElement == null)
                {
                    result.Actual = TestStatus.Failed;
                    result.OutputMessage = "There is no element using the settings given in this test case";
                    return result;
                }
                switch(this.Action)
                {
                    case ElementAction.SendKeys:
                        this.Keys = this.Keys.Evaluate().ToString();
                        element.ClearFirstSendKeys(Keys.ToSeleniumKeys());
                        break;
                    case ElementAction.Click:
                        element.Click();
                        break;
                    default:
                        result.Description = "No action performed. Use element properties.";
                        break;

                }
                result.Actual = TestStatus.Passed;
            }
            catch (Exception ex)
            {
                result.Actual = TestStatus.Failed;
                result.OutputMessage = ex.Message;
            }

            
            return result;
        }

        public new static System.Windows.Media.ImageSource Icon
        {
            get
            {
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Action);
            }
        }

        public override BaseWebTest BaseUI
        {
            get
            {
                BaseWebTest c = base.BaseUI;
                c.WebContentControl = new ActionTestUI();
                return c;
            }
        }
        #endregion
    }
}

