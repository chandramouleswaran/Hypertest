﻿#region License

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

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Expression")]
    [Description("Evaluate an expression")]
    [Category("General")]
    [TestImage("Images/Expression.png")]
    public class ExpressionTestCase : TestCase
    {
        #region Members

        private string _expression;
        private object _expressionValue;
        private string _expectedValue;

        #endregion

        #region CTOR

        public ExpressionTestCase()
        {
            Initialize();
        }

        private void Initialize(bool create = true)
        {
            this.Description = "Evaluates the expression";
            this.MarkedForExecution = true;
        }

        #endregion

        #region Property

        [DataMember]
        [DisplayName("Expression")]
        [Description("Enter the expression to evaluate - use variables in the format (%VARIABLE%)")]
        [DynamicReadonly("RunState")]
        [Category("Settings")]
        public string Expression
        {
            get { return _expression; }
            set
            {
                string oldValue = _expression;
                _expression = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _expression, "Expression change");
            }
        }

        [DataMember]
        [DisplayName("Expected value")]
        [Description("Enter the expected value the expression should evaluate to")]
        [DynamicReadonly("RunState")]
        [Category("Settings")]
        public string ExpectedValue
        {
            get { return _expectedValue; }
            set
            {
                string oldValue = _expectedValue;
                _expectedValue = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _expectedValue, "Expected value change");
            }
        }

        [DataMember]
        [DisplayName("Evaluated Value")]
        [Description("The evaluated value of the expression")]
        [DynamicReadonly("RunState"), DynamicBrowsable("RunState")]
        [Category("Settings")]
        public object Value
        {
            get { return _expressionValue; }
            set
            {
                object oldValue = _expressionValue;
                _expressionValue = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _expressionValue, "Expression Value change");
            }
        }

        #endregion

        #region Deserialize

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        #endregion

        #region Override

        public override void Body()
        {
            this.ActualResult = TestCaseResult.Passed;
            try
            {
                this.Value = this.Expression.Evaluate();
                if (this.Value != null && !string.IsNullOrEmpty(this.ExpectedValue))
                {
                    if (this.Value.ToString() == this.ExpectedValue)
                    {
                        this.ActualResult = TestCaseResult.Passed;
                        //result.OutputMessage += "Expression evaluated to the expected value";
                        //result.Actual = TestStatus.Passed;
                    }
                    else
                    {
                        this.ActualResult = TestCaseResult.Failed;
                        //result.Actual = TestStatus.Failed;
                        //result.OutputMessage += "Expression evaluated to: " + this.Value + "\n";
                        //result.OutputMessage += "Expression expected: " + this.ExpectedValue + "\n";
                    }
                }
                else
                {
                    if (this.ExpectedValue == "null")
                    {
                        this.ActualResult = TestCaseResult.Passed;
                    }
                    else if (string.IsNullOrEmpty(this.ExpectedValue))
                    {
                        //result.Description = "Expression evaluated. No comparison performed.";
                        //result.Actual = TestStatus.Passed;
                        this.ActualResult = TestCaseResult.Passed;
                    }
                    else
                    {
                        //result.Actual = TestStatus.Failed;
                        //result.OutputMessage += "Expression evaluated to null";
                        this.ActualResult = TestCaseResult.Failed;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}