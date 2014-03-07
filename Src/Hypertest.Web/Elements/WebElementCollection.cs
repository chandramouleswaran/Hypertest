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
using System.Collections.ObjectModel;
using Hypertest.Web.Utils;
using OpenQA.Selenium;

namespace Hypertest.Web.Elements
{
	/// <summary>
	/// Base web element
	/// </summary>
	public class WebElementCollection : List<WebElement>
	{
		public WebElementCollection()
		{
		}

		public WebElementCollection(IWebDriver webDriver, By by, bool recurse = false)
		{
			try
			{
				if (recurse)
				{
					List<IWebElement> elements = SearchElement(webDriver, by);
					foreach (var element in elements)
					{
						this.Add(new WebElement(element));
					}
				}
				else
				{
					var tempElements = webDriver.FindElements(by);
					foreach (IWebElement element in tempElements)
					{
						this.Add(new WebElement(element));
					}
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

		private List<IWebElement> SearchElement(IWebDriver webDriver, By by)
		{
			List<IWebElement> elementList = new List<IWebElement>(webDriver.FindElements(by));
			ReadOnlyCollection<IWebElement> frames = webDriver.FindElements(By.XPath("//iframe"));
			foreach (IWebElement frame in frames)
			{
				webDriver.SwitchTo().Frame(frame);
				List<IWebElement> innerElements = SearchElement(webDriver, by);
				elementList.AddRange(innerElements);
				webDriver.SwitchTo().DefaultContent();
				webDriver.SwitchTo().Frame(0);
			}
			return elementList;
		}
	}
}