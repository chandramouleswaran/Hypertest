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
    /// Frame switch test case
    /// </summary>
    [Serializable]
    [DisplayName("Switch Frame")]
    public class FrameSwitchTestCase : WebTestCase
    {
        #region Property
        public bool BaseFrame { get; set; }
        public string FrameDescriptor { get; set; }
        #endregion

        #region CTOR
        public FrameSwitchTestCase():base()
        {
            this.Name = "Switch frame test";
            this.Expected = TestStatus.Passed;
            this.WaitTime = 2000;
            this.BaseFrame = false;
            this.FrameDescriptor = "0";
        }

        public FrameSwitchTestCase(String description)
            : this()
        {
            this.Description = description;
        }
        #endregion

        #region Overrides
        public override bool IsParent
        {
            get
            {
                return false;
            }
        }

        public override TestResult Run()
        {
            IWebDriver driver = Runner.Instance.Driver;
            result = base.Run();
            driver.SwitchTo().DefaultContent();
            try
            {
                if (this.BaseFrame)
                {
                    driver.SwitchTo().DefaultContent();
                }
                else
                {
                    int intVal;
                    if(int.TryParse(this.FrameDescriptor, out intVal))
                    {
                        driver.SwitchTo().Frame(intVal);
                    }
                    else
                    {
                        driver.SwitchTo().Frame(this.FrameDescriptor);
                    }
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
                return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Frame);
            }
        }

        public override BaseWebTest BaseUI
        {
            get
            {
                BaseWebTest c = base.BaseUI;
                c.WebContentControl = new FrameSwitchUI();
                return c;
            }
        }
        #endregion
    }
}

