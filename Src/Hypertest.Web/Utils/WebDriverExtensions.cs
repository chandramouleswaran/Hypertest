#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Hypertest.Web.Elements;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Hypertest.Web.Utils
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

        public static IEnumerable<IWebElement> FindElements(this IWebElement driver, By by,
            Func<WebElement, bool> predicate)
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

        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, By by,
            Func<WebElement, bool> predicate)
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
            if (element.GetType() == typeof (RemoteWebElement))
            {
                return driver.SwitchTo().ActiveElement().Equals(element);
            }

            if (element.GetType() == typeof (WebElement))
            {
                WebElement wElement = element as WebElement;
                return wElement != null && driver.SwitchTo().ActiveElement().Equals(wElement.InnerElement);
            }
            return false;
        }
    }
}