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
using System.Collections.Generic;

using OpenQA.Selenium;

namespace Hypertest.WebTest.Elements
{
    /// <summary>
    /// Base web element
    /// </summary>
    public class WebElementCollection : List<WebElement>
    {
        public WebElementCollection()
        {
        }

        public WebElementCollection(IWebDriver webDriver, By by)
        {
            try
            {
                var tempElements = webDriver.FindElements(by);

                foreach (IWebElement element in tempElements)
                {
                    this.Add(new WebElement(element));
                }
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElementCollection(IWebElement webElement, By by)
        {
            try
            {
                var tempElements = webElement.FindElements(by);

                foreach (IWebElement element in tempElements)
                {
                    this.Add(new WebElement(element));
                }
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElementCollection(IWebDriver webDriver, By by, Func<IWebElement, bool> predicate)
        {
            try
            {
                var tempElements = webDriver.FindElements(by, predicate);

                foreach (IWebElement element in tempElements)
                {
                    this.Add(new WebElement(element));
                }
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElementCollection(IWebElement webElement, By by, Func<IWebElement, bool> predicate)
        {
            try
            {
                var tempElements = webElement.FindElements(by, predicate);

                foreach (IWebElement element in tempElements)
                {
                    this.Add(new WebElement(element));
                }
            }
            catch (NoSuchElementException)
            {
            }
        }
    }
}
