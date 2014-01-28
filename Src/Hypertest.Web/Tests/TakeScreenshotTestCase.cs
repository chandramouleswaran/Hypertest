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
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using Hypertest.Core.Attributes;
using Hypertest.Core.Runners;
using Hypertest.Core.Utils;
using Hypertest.Web.Tests;
using OpenQA.Selenium;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Take Screenshot")]
    [Description("Takes a screenshot of the current state of browser")]
    [Category("Web")]
    [TestImage("Images/Screenshot.png")]
    public class TakeScreenshotTestCase : TestCase
    {
        #region Members
        private string _screenshotPath;
        #endregion

        #region CTOR
		public TakeScreenshotTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            this.Description = "Take a screenshot";
            this.MarkedForExecution = true;
        }

        #endregion

        #region Deserialize

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        #endregion

		#region Property
		/// <summary>
		/// Gets or sets the value used along with query type.
		/// </summary>
		[DataMember]
		[DisplayName("Screenshot")]
		[Description("The file path to the screenshot")]
		[Category("Settings")]
		[DynamicReadonly("RunState")]
		[PostRun]
		public String ScreenshotPath
		{
			get { return _screenshotPath; }
			set
			{
				string oldValue = _screenshotPath;
				_screenshotPath = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _screenshotPath, "Screenshot location change");
			}
		} 
		#endregion

        #region Override
        protected override void Body()
        {
            try
            {
				this.ActualResult = TestCaseResult.Passed;
				this.ScreenshotPath = WebScenarioRunner.Current.RunFolder + Path.DirectorySeparatorChar + DateTime.Now.Ticks.ToString() + ".png";
				((ITakesScreenshot)WebScenarioRunner.Current.Driver).GetScreenshot().SaveAsFile(this.ScreenshotPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                this.Log(ex.Message, LogCategory.Exception, LogPriority.High);
                this.Log(ex.StackTrace, LogCategory.Exception, LogPriority.High);
                this.ActualResult = TestCaseResult.Failed;
            }
        }
        #endregion
    }
}