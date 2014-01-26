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
using System.Threading;
using Hypertest.Web.Utils;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium;

namespace Hypertest.Web.Elements
{
    /// <summary>
    /// Base web element
    /// </summary>
    [Serializable]
    public class WebElement : IWebElement
    {
        #region CTOR 

        public WebElement()
        {
        }

        public WebElement(IWebElement webElement)
        {
            InnerElement = webElement;
            InitializeJavaScriptExecutor();
        }

        public WebElement(IWebDriver webDriver, By by)
        {
            try
            {
                InnerElement = webDriver.FindElement(by);
                InitializeJavaScriptExecutor();
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElement(IWebElement webElement, By by)
        {
            try
            {
                InnerElement = webElement.FindElement(by);
                InitializeJavaScriptExecutor();
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElement(IWebDriver webDriver, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                InnerElement = webDriver.FindElement(by, predicate);
                InitializeJavaScriptExecutor();
            }
            catch (NoSuchElementException)
            {
            }
        }

        public WebElement(IWebElement webElement, By by, Func<WebElement, bool> predicate)
        {
            try
            {
                InnerElement = webElement.FindElement(by, predicate);
                InitializeJavaScriptExecutor();
            }
            catch (NoSuchElementException)
            {
            }
        }

        #endregion

        #region Property

        public bool Displayed
        {
            get
            {
                if (InnerElement != null)
                {
                    return InnerElement.Displayed;
                }
                return false;
            }
        }

        public bool Exists
        {
            get
            {
                try
                {
                    if (InnerElement != null)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string ClassName
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("class");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Name
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("name");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string ID
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("id");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Style
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("style");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Value
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("value");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Type
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("type");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Title
        {
            get
            {
                try
                {
                    return InnerElement.GetAttribute("title");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public WebElement Parent
        {
            get
            {
                try
                {
                    return new WebElement(InnerElement, By.XPath("./parent::*"));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public WebElement Child
        {
            get
            {
                try
                {
                    return new WebElement(InnerElement, By.XPath("./child::*"));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        // public GetWebElement PreviousSibling
        //{
        //    get
        //    {
        //        return new GetWebElement(GetWebElement, By.XPath("./preceding-sibling::*"));
        //    }
        //}

        public WebElement NextSibling
        {
            get
            {
                try
                {
                    return new WebElement(InnerElement, By.XPath("./following-sibling::*"));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool Enabled
        {
            get
            {
                try
                {
                    return InnerElement.Enabled;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public System.Drawing.Point Location
        {
            get { return InnerElement.Location; }
        }

        public bool Selected
        {
            get { return InnerElement.Selected; }
        }

        public System.Drawing.Size Size
        {
            get { return InnerElement.Size; }
        }

        public virtual string ElementTag
        {
            get { return string.Empty; }
        }

        public string TagName
        {
            get { return InnerElement.TagName; }
        }

        public string Text
        {
            get
            {
                try
                {
                    return InnerElement.Text;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private IJavaScriptExecutor JavaScriptExecuter { get; set; }
        protected IWebDriver ElementsWebDriver { get; set; }

        public WebElementCollection Elements
        {
            get { return new WebElementCollection(InnerElement, By.CssSelector("*")); }
        }

        public WebElementCollection Buttons
        {
            get
            {
                return new WebElementCollection(InnerElement, By.TagName("input"),
                    i => i.GetAttribute("type") == "button");
            }
        }

        public WebElementCollection CheckBoxes
        {
            get
            {
                return new WebElementCollection(InnerElement, By.TagName("input"),
                    i => i.GetAttribute("type") == "checkbox");
            }
        }

        public WebElementCollection Divs
        {
            get { return new WebElementCollection(InnerElement, By.TagName("div")); }
        }

        public WebElementCollection Images
        {
            get { return new WebElementCollection(InnerElement, By.TagName("img")); }
        }

        public WebElementCollection Labels
        {
            get { return new WebElementCollection(InnerElement, By.TagName("label")); }
        }

        public WebElementCollection Links
        {
            get { return new WebElementCollection(InnerElement, By.TagName("a")); }
        }

        public WebElementCollection RadioButtons
        {
            get
            {
                return new WebElementCollection(InnerElement, By.TagName("input"),
                    i => i.GetAttribute("type") == "radio");
            }
        }

        public WebElementCollection SelectLists
        {
            get { return new WebElementCollection(InnerElement, By.TagName("select")); }
        }

        public WebElementCollection Spans
        {
            get { return new WebElementCollection(InnerElement, By.TagName("span")); }
        }

        public WebElementCollection TableBodies
        {
            get { return new WebElementCollection(InnerElement, By.TagName("tbody")); }
        }

        public WebElementCollection TableBodyCells
        {
            get { return new WebElementCollection(InnerElement, By.TagName("td")); }
        }

        public WebElementCollection TableHeadCells
        {
            get { return new WebElementCollection(InnerElement, By.TagName("td")); }
        }

        public WebElementCollection TableHeads
        {
            get { return new WebElementCollection(InnerElement, By.TagName("thead")); }
        }

        public WebElementCollection TableRows
        {
            get { return new WebElementCollection(InnerElement, By.TagName("tr")); }
        }

        public WebElementCollection Tables
        {
            get { return new WebElementCollection(InnerElement, By.TagName("table")); }
        }

        public WebElementCollection TextFields
        {
            get
            {
                return new WebElementCollection(InnerElement, By.TagName("input"), i => i.GetAttribute("type") == "text");
            }
        }


        public IWebElement InnerElement { get; set; }

        #endregion

        #region Methods

        public WebElement Element(Predicate<WebElement> predicate)
        {
            try
            {
                return Elements.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Button(Predicate<WebElement> predicate)
        {
            try
            {
                return Buttons.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement CheckBox(Predicate<WebElement> predicate)
        {
            try
            {
                return CheckBoxes.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Div(Predicate<WebElement> predicate)
        {
            try
            {
                return Divs.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Image(Predicate<WebElement> predicate)
        {
            try
            {
                return Images.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Label(Predicate<WebElement> predicate)
        {
            try
            {
                return Labels.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Link(Predicate<WebElement> predicate)
        {
            try
            {
                return Links.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement RadioButton(Predicate<WebElement> predicate)
        {
            try
            {
                return RadioButtons.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement SelectList(Predicate<WebElement> predicate)
        {
            try
            {
                return SelectLists.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Span(Predicate<WebElement> predicate)
        {
            try
            {
                return Spans.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement Table(Predicate<WebElement> predicate)
        {
            try
            {
                return Tables.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WebElement TextField(Predicate<WebElement> predicate)
        {
            try
            {
                return TextFields.Find(predicate);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ExecuteScript(string script, params object[] args)
        {
            JavaScriptExecuter.ExecuteScript(script, args);
        }

        public void ExecuteAsyncScript(string script, params object[] args)
        {
            JavaScriptExecuter.ExecuteAsyncScript(script, args);
        }

        public T ConvertTo<T>() where T : WebElement
        {
            var instance = Activator.CreateInstance(typeof (T), new object[] {InnerElement}) as T;

            return instance;
        }

        public void Focus()
        {
            JavaScriptExecuter.ExecuteScript("arguments[0].focus();", InnerElement);
        }

        public void Blur()
        {
            JavaScriptExecuter.ExecuteScript("arguments[0].blur();", InnerElement);
        }

        public WebElement FindWebElement(By by)
        {
            return new WebElement(InnerElement.FindElement(by));
        }

        public IWebElement FindElement(By by)
        {
            return InnerElement.FindElement(by);
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return InnerElement.FindElements(by);
        }

        public void Clear()
        {
            InnerElement.Clear();
        }

        public void Click()
        {
            InnerElement.Click();
        }

        public string GetAttribute(string attributeName)
        {
            return InnerElement.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return InnerElement.GetCssValue(propertyName);
        }

        public void SendKeys(string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }

            InnerElement.SendKeys(text);
        }

        public void ClearFirstSendKeys(string text)
        {
            if (text == null)
            {
                text = string.Empty;
            }

            InnerElement.Clear();
            InnerElement.SendKeys(text);
        }

        public void Submit()
        {
            InnerElement.Submit();
        }

        public void WaitUntilVisible(int timeoutTime = 5000)
        {
            int tempTime = 0;
            while (!InnerElement.Displayed && tempTime <= timeoutTime)
            {
                Thread.Sleep(100);
                tempTime += 100;
            }
        }

        public void WaitUntilExists(int timeoutTime = 5000)
        {
            int tempTime = 0;
            while (!Exists && tempTime <= timeoutTime)
            {
                Thread.Sleep(100);
                tempTime += 100;
            }
        }

        private void InitializeJavaScriptExecutor()
        {
            try
            {
                var wrappedElement = (IWrapsDriver) InnerElement;
                ElementsWebDriver = wrappedElement.WrappedDriver;
                JavaScriptExecuter = (IJavaScriptExecutor) ElementsWebDriver;
            }
            catch (Exception)
            {
                ElementsWebDriver = null;
                JavaScriptExecuter = null;
            }
        }

        public bool HasFocus(IWebDriver driver)
        {
            return InnerElement.HasFocus(driver);
        }

        #endregion
    }
}