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
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

using Hypertest.WebTest.Elements;

namespace Hypertest.WebTest
{
    public static class DriverExtensions
    {
        public static IWebElement FindElement(this IWebElement driver, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                return new WebElementCollection(driver, by).Where(predicate).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<IWebElement> FindElements(this IWebElement driver, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                return new WebElementCollection(driver, by).Where(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                return new WebElementCollection(driver, by).Where(predicate).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                return new WebElementCollection(driver, by).Where(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool HasFocus(this IWebElement element, IWebDriver driver)
        {
            if (element.GetType() == typeof(RemoteWebElement))
            {
                return driver.SwitchTo().ActiveElement().Equals(element);
            }

            if (element.GetType() == typeof(WebElement))
            {
                WebElement wElement = element as WebElement;
                return wElement != null && driver.SwitchTo().ActiveElement().Equals(wElement.InnerElement);
            }
            return false;
        }
    }
}