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
using Hypertest.Core.Utils;
using Hypertest.Web.Tests;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Send Keys")]
    [Description("Sends simple keys to a web element on the browser")]
    [Category("Web")]
    [TestImage("Images/SendKeys.png")]
    [ScenarioTypes(typeof(WebTestScenario))]
    public class SendKeysTestCase : WebTestCase
    {
        #region Members

        private string _keys;
        private bool _clearField;

        #endregion

        #region CTOR

        public SendKeysTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            this.Description = "Send keys to a web element";
            this.MarkedForExecution = true;
            this.Keys = "SACHIN TENDULKAR{ENTER}";
            this.ClearField = true;
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
        [DisplayName("Keys to send")]
        [Description("Enter the keys to send to the element")]
        [Category("Settings")]
        [DynamicReadonly("RunState")]
        public String Keys
        {
            get { return _keys; }
            set
            {
                string oldValue = _keys;
                _keys = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _keys, "Keys change");
            }
        }

        [DataMember]
        [DisplayName("Clear the element?")]
        [Description("Do you want to clear the field before sending key strokes?")]
        [Category("Settings")]
        [DynamicReadonly("RunState")]
        public bool ClearField
        {
            get { return _clearField; }
            set
            {
                bool oldValue = _clearField;
                _clearField = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _clearField, "Clear Field change");
            }
        }
        #endregion

        #region Override
        protected override void Body()
        {
            base.Body();
            if (this.ActualResult == TestCaseResult.Failed) return;

            //We have reached so far - this means we have an element
            try
            {
                this.Keys = StringExtensions.Evaluate(this.Keys, this.Runner).ToString();
                if (this.ClearField)
                {
                    this.Element.ClearFirstSendKeys(Keys.ToSeleniumKeys());
                }
                else
                {
                    this.Element.SendKeys(Keys.ToSeleniumKeys());
                }
                this.ActualResult = TestCaseResult.Passed;
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