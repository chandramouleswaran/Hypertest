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
using System.Runtime.Serialization;
using Hypertest.Core.Attributes;
using Hypertest.Core.Runners;
using Hypertest.Web.Tests;
using OpenQA.Selenium;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Switch Frame")]
    [Description("Selects the \"iframe\" on which the rest of the actions should take place")]
    [Category("Web")]
    [TestImage("Images/Frame.png")]
    public class SwitchFrameTestCase : TestCase
    {
        #region Member
		private bool _baseFrame;
        private string _frameDescriptor; 
	    #endregion

        #region CTOR
        public SwitchFrameTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            this.Description = "Click a particular web element";
            this.MarkedForExecution = true;
            this.BaseFrame = false;
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
        [DataMember]
        [DisplayName("Default Content?")]
        [Description("Switches to the default content")]
        [Category("Settings")]
        [DynamicReadonly("RunState")]
        public bool BaseFrame
        {
            get { return _baseFrame; }
            set
            {
                bool oldValue = _baseFrame;
                _baseFrame = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _baseFrame, "Base Frame change");
            }
        }

        [DataMember]
        [DisplayName("Frame Descriptor")]
        [Description("Enter frame number or name to switch to")]
        [Category("Settings")]
        [DynamicReadonly("RunState")]
        public string FrameDescriptor
        {
            get { return _frameDescriptor; }
            set
            {
                string oldValue = _frameDescriptor;
                _frameDescriptor = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _frameDescriptor, "Frame Descriptor change");
            }
        }
        #endregion

        #region Override
        protected override void Body()
        {
            this.ActualResult = TestCaseResult.Passed;
            IWebDriver driver = WebScenarioRunner.Current.Driver;
            try
            {
                if (this.BaseFrame)
                {
                    driver.SwitchTo().DefaultContent();
                }
                else
                {
                    //Might be an integer - try and parse it and switch to that iframe
                    int intVal;
                    if (int.TryParse(this.FrameDescriptor, out intVal))
                    {
                        driver.SwitchTo().Frame(intVal);
                    }
                    else
                    {
                        driver.SwitchTo().Frame(this.FrameDescriptor);
                    }
                }
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