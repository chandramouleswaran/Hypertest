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
using Hypertest.Core.Interfaces;
using Hypertest.Core.Runners;

namespace Hypertest.Core.Tests
{
    /// <summary>
    ///     The basic unit of a web test scenario
    /// </summary>
    [DataContract]
    [Serializable]
    [DisplayName("Web test scenario")]
    [TestImage("Images/Scenario.png")]
    public class WebTestScenario : TestScenario
    {
        #region Members
        private string _url;
        private BrowserType _type; 
        #endregion

        #region CTOR
        public WebTestScenario() : base()
        {
            this.Description = "Enter Web Scenario Description Here";
        } 
        #endregion

        #region Property
        [DataMember]
        [Category("Scenario Settings")]
        [DisplayName("Starting URL")]
        [Description("Enter the URL")]
        [DynamicReadonly("RunState")]
        public string URL
        {
            get { return _url; }
            set
            {
                string oldValue = _url;
                if (oldValue != value)
                {
                    _url = value;
                    RaisePropertyChangedWithValues(oldValue, value, "URL change");
                }
            }
        }

        [DataMember]
        [Category("Scenario Settings")]
        [DisplayName("Browser Type")]
        [Description("Select the browser type")]
        [DynamicReadonly("RunState")]
        public BrowserType BrowserType
        {
            get { return _type; }
            set
            {
                BrowserType oldValue = _type;
                if (oldValue != value)
                {
                    _type = value;
                    RaisePropertyChangedWithValues(oldValue, value, "Type change");
                }
            }
        } 
        #endregion

        #region Override
        protected override void Setup()
        {
            foreach (Variable variable in Variables)
            {
                WebScenarioRunner.Current.AddVariable(variable);
            }
        } 
        #endregion
    }
}