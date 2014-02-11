#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using Hypertest.Core.Interfaces;

namespace Hypertest.Core.Utils
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the selenium keys for this string
        /// Form keys - http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>The selenium strings</returns>
        public static string ToSeleniumKeys(this string keys)
        {
            keys = keys.Replace("{BACKSPACE}", OpenQA.Selenium.Keys.Backspace);
            keys = keys.Replace("{BS}", OpenQA.Selenium.Keys.Backspace);
            keys = keys.Replace("{BKSP}", OpenQA.Selenium.Keys.Backspace);
            keys = keys.Replace("{DELETE}", OpenQA.Selenium.Keys.Delete);
            keys = keys.Replace("{DEL}", OpenQA.Selenium.Keys.Delete);
            keys = keys.Replace("{DOWN}", OpenQA.Selenium.Keys.Down);
            keys = keys.Replace("{END}", OpenQA.Selenium.Keys.End);
            keys = keys.Replace("{ENTER}", OpenQA.Selenium.Keys.Enter);
            keys = keys.Replace("{RETURN}", OpenQA.Selenium.Keys.Return);
            keys = keys.Replace("~", OpenQA.Selenium.Keys.Enter);
            keys = keys.Replace("{ESC}", OpenQA.Selenium.Keys.Escape);
            keys = keys.Replace("{HELP}", OpenQA.Selenium.Keys.Help);
            keys = keys.Replace("{HOME}", OpenQA.Selenium.Keys.Home);
            keys = keys.Replace("{INS}", OpenQA.Selenium.Keys.Insert);
            keys = keys.Replace("{INSERT}", OpenQA.Selenium.Keys.Insert);
            keys = keys.Replace("{LEFT}", OpenQA.Selenium.Keys.Left);
            keys = keys.Replace("{PGDN}", OpenQA.Selenium.Keys.PageDown);
            keys = keys.Replace("{PGUP}", OpenQA.Selenium.Keys.PageUp);
            keys = keys.Replace("{RIGHT}", OpenQA.Selenium.Keys.Right);
            keys = keys.Replace("{TAB}", OpenQA.Selenium.Keys.Tab);
            keys = keys.Replace("{UP}", OpenQA.Selenium.Keys.Up);
            keys = keys.Replace("{F1}", OpenQA.Selenium.Keys.F1);
            keys = keys.Replace("{F2}", OpenQA.Selenium.Keys.F2);
            keys = keys.Replace("{F3}", OpenQA.Selenium.Keys.F3);
            keys = keys.Replace("{F4}", OpenQA.Selenium.Keys.F4);
            keys = keys.Replace("{F5}", OpenQA.Selenium.Keys.F5);
            keys = keys.Replace("{F6}", OpenQA.Selenium.Keys.F6);
            keys = keys.Replace("{F7}", OpenQA.Selenium.Keys.F7);
            keys = keys.Replace("{F8}", OpenQA.Selenium.Keys.F8);
            keys = keys.Replace("{F9}", OpenQA.Selenium.Keys.F9);
            keys = keys.Replace("{F10}", OpenQA.Selenium.Keys.F10);
            keys = keys.Replace("{F11}", OpenQA.Selenium.Keys.F11);
            keys = keys.Replace("{F12}", OpenQA.Selenium.Keys.F12);
            return keys;
        }

        public static object Evaluate(string text, IRunner runner, bool replace = true)
        {
            return EvalCode.Evaluate(text, runner, replace);
        }
    }
}