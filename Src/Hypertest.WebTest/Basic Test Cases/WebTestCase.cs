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
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Runtime.Serialization;
using OpenQA.Selenium;

using Hypertest.Core;
using Hypertest.Core.GUI;
using Hypertest.Core.Utils;
using Hypertest.WebTest.Elements;

namespace Hypertest.WebTest
{
    /// <summary>
    /// Web test case
    /// </summary>
    [Serializable]
    [TestIgnore]
    public class WebTestCase : TestCase
    {
        #region CTOR
        public WebTestCase():base()
        {
            this.WaitTime = 1000;
            this.What = ElementQueryType.ElementID;
            this.ElementNumber = 1;
            this.ParentVariable = "";
        }
        #endregion

        #region Members
        [XmlIgnore]
        protected By by;
        [XmlIgnore]
        protected WebElement element;
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the parent variable which has a web element
        /// </summary>
        public string ParentVariable { get; set; }

        /// <summary>
        /// Gets or sets ElementNumber.
        /// </summary>
        public int ElementNumber { get; set; }

        /// <summary>
        /// Gets or sets the value used along with query type.
        /// </summary>
        public String ElementDescriptor { get; set; }
        
        /// <summary>
        /// Gets or sets how to query the element.
        /// </summary>
        public ElementQueryType What { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the element should be cleared before the test case executes
        /// </summary>
        public bool Clear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show base content.
        /// </summary>
        /// <value>
        ///   <c>true</c> if you want to show base content; otherwise, <c>false</c>.
        /// </value>
        public bool ShowBaseContent { get; set; }

        /// <summary>
        /// Gets the web element after the run and can be stored in a variable.
        /// </summary>
        [XmlIgnore]
        public virtual WebElement QueriedElement
        {
            get
            {
                return element;
            }
        }

        [XmlIgnore]
        public virtual BaseWebTest BaseUI
        {
            get
            {
                return new BaseWebTest();
            }
        }

        [XmlIgnore]
        protected By ElementBy
        {
            get
            {
                switch (What)
                {
                    case ElementQueryType.ElementID:
                        by = By.Id(ElementDescriptor);
                        break;
                    case ElementQueryType.ElementName:
                        by = By.Name(ElementDescriptor);
                        break;
                    case ElementQueryType.LinkText:
                        by = By.PartialLinkText(ElementDescriptor);
                        break;
                    case ElementQueryType.XPath:
                        by = By.XPath(ElementDescriptor);
                        break;
                    case ElementQueryType.ClassName:
                        by = By.ClassName(ElementDescriptor);
                        break;
                    case ElementQueryType.CssSelector:
                        by = By.CssSelector(ElementDescriptor);
                        break;
                    case ElementQueryType.TagName:
                        by = By.TagName(ElementDescriptor);
                        break;
                    default:
                        by = null;
                        break;
                }
                return by;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the web element based on the query.
        /// </summary>
        /// <param name="driver">The web driver.</param>
        /// <returns></returns>
        protected WebElement GetWebElement(IWebDriver driver)
        {
            By by = ElementBy;
            if (by != null)
            {
                if (ElementNumber <= 0)
                {
                    ElementNumber = 1;
                }
                if (string.IsNullOrEmpty(this.ParentVariable))
                {
                    WebElementCollection c = new WebElementCollection(driver, ElementBy);
                    this.TotalElements = c.Count;
                    element = c[ElementNumber - 1];
                }
                else
                {
                    WebElement w = Runner.Instance.GetVariable(this.ParentVariable.Replace("%","")).Value as WebElement;
                    WebElementCollection c = new WebElementCollection(w, ElementBy);
                    this.TotalElements = c.Count;
                    element = c[ElementNumber - 1];
                }
            }
            else
            {
                Variable v = Runner.Instance.GetVariable(ElementDescriptor);
                if (v != null)
                {
                    element = v.Value as WebElement;
                    if (element == null)
                    {
                        result.Description = "Unable to find a web element using the variable.";
                    }
                }
                else
                {
                    result.Description = "Unable to find the variable.";
                }
            }
            return element;
        }
        #endregion

        #region Post Run
        [PostRun]
        [XmlIgnore]
        public virtual string ClassName
        {
            get { return element.ClassName; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual bool Displayed
        {
            get { return element.Displayed; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual bool IsEnabled
        {
            get { return element.Enabled; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual bool Exists
        {
            get { return element.Exists; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual bool HasFocus
        {
            get { return element.HasFocus(Runner.Instance.Driver); }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string ID
        {
            get { return element.Id; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual int LocationX
        {
            get { return element.Location.X; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual int LocationY
        {
            get { return element.Location.Y; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string ElementName
        {
            get { return element.Name; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual int Height
        {
            get { return element.Size.Height; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual int Width
        {
            get { return element.Size.Width; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string Style
        {
            get { return element.Style; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string Text
        {
            get { return element.Text; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string Title
        {
            get { return element.Title; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string Value
        {
            get { return element.Value; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual string TagName
        {
            get { return element.TagName; }
        }
        [PostRun]
        [XmlIgnore]
        public virtual int TotalElements { get; private set; }
        [PostRun]
        [XmlIgnore]
        public virtual WebElement WebElement
        {
            get
            {
                return element;
            }
        }
        #endregion

        #region Overrides
        public new UserControl Control
        {
            get
            {
                TestEditControl c = base.Control;
                c.TestSpecificControl = this.BaseUI;
                return c;
            }
        }
        #endregion
    }
}
