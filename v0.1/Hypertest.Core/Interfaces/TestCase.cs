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
using System.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Reflection;
using OpenQA.Selenium;

using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.Core
{
    /// <summary>
    /// The base class for all test cases p
    /// </summary>
    [Serializable]
    public class TestCase
    {
        #region CTOR
        public TestCase()
        {
            WaitTime = 0;
            CanResult = true;
            IsParent = false;
            children = new ObservableCollection<TestCase>();
            this.PostValues = new ObservableCollection<PostRunPairs>();
        }
        #endregion

        #region Virtuals
        /// <summary>
        /// Initialize the test case - initialize variables, use runner if needed
        /// </summary>
        /// <param name="runner">The instance of the runner executing the test case</param>
        public virtual void Initialize(ITestRunner runner) { }

        /// <summary>
        /// Set the parent test case of the current test case
        /// </summary>
        /// <param name="parent">The parent test case</param>
        public virtual void SetParent(TestCase parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Generates the proper test result required by the test case
        /// </summary>
        public virtual TestResult GenerateResult()
        {
            if (this.result == null)
            {
                this.result = new TestResult(this) { Status = TestStatus.Executing };
            }
            return this.result;
        }

        internal virtual void ClearResults()
        {
            if (result != null && result.Children != null)
            {
                result.Children.Clear();
            }
            foreach (TestCase child in Children)
            {
                child.ClearResults();
            }
            result = null;
        }

        /// <summary>
        /// Cleans up the mess which this test case created once the run is over
        /// </summary>
        public virtual void CleanUp()
        {
            foreach (TestCase child in Children)
            {
                child.CleanUp();
            }
        }

        /// <summary>
        /// Executes the test case
        /// </summary>
        public virtual TestResult Run() 
        {
            ITestRunner runner = Runner.Instance;
            bool failed = false;

            if (runner.IsRunning == false)
            {
                return null;
            }

            result = GenerateResult();

            if (parent != null)
            {
                parent.CurrentResult.AddChild(result);
            }

            runner.Refresh();

            foreach (TestCase test in children)
            {
                if (!test.ExcludeFromExecution)
                {
                    //Initialize
                    test.Initialize(runner);
                    
                    //Run
                    test.SetParent(this);
                    try
                    {
                        TestResult childResult = test.Run();
                        test.PostRun();
                        
                        //Wait
                        test.Wait();

                        //Check for the conditions and apply the required value
                        childResult.Status = CheckCondition(test, childResult);

                        if (failed == false)
                        {
                            failed = (childResult.Status == TestStatus.Failed);
                        }

                        //Stop runner on exit
                        if (childResult.Status == TestStatus.Failed && test.ExitOnError)
                        {
                            Runner.Instance.Stop();
                            break;
                        }

                        //Exit branch on a failure
                        if (childResult.Status == TestStatus.Failed && test.ExitBranchOnError)
                        {
                            break;
                        }
                    }
                    catch (Exception exception)
                    {
                        result.Description = exception.Message;
                        result.Description += "\n" + exception.StackTrace;
                    }
                }
            }

            if (children.Count > 0)
            {
                this.result.Actual = !failed ? TestStatus.Passed : TestStatus.Failed;
            }

            result.Status = CheckCondition(this,result);
            runner.Refresh();
            return result;
        }

        public virtual void Wait() 
        {
            Runner.Instance.Wait(this.WaitTime);
        }

        /// <summary>
        /// Check the condition
        /// </summary>
        protected TestStatus CheckCondition(TestCase c, TestResult r) 
        {
            TestStatus stat = TestStatus.None;

            if (c.CanResult)
            { 
                stat = c.Expected == r.Actual ? TestStatus.Passed : TestStatus.Failed;
            }
            return stat;
        }

        /// <summary>
        /// Destroys the test case
        /// </summary>
        public virtual void Destroy() 
        {
            foreach (TestCase child in children)
            {
                child.Destroy();
            }
            ClearResults();
        }

        /// <summary>
        /// Post run computations for this test
        /// </summary>
        internal virtual void PostRun()
        {
            if (TakeScreenShot)
            {
                //Check for the attributes and take screenshot approriately
                this.result.ScreenShotLocation = FileUtils.AppPath + Path.DirectorySeparatorChar + Runner.Instance.UniqueID + Path.DirectorySeparatorChar + DateTime.Now.Ticks.ToString() + ".png";
                ((ITakesScreenshot)Runner.Instance.Driver).GetScreenshot().SaveAsFile(this.result.ScreenShotLocation, System.Drawing.Imaging.ImageFormat.Png);
            }

            //Reflect the property and store the value
            foreach (PostRunPairs p in PostValues.Where(f => !string.IsNullOrEmpty(f.VariableName)))
            {
                PropertyInfo property = this.GetType().GetProperties().First(prop => prop.IsDefined(typeof(PostRun), false) && prop.Name.Equals(p.PropertyName));
                if (property != null)
                {
                    try
                    {
                        Object val = property.GetValue(this, null);
                        if (val != null)
                        {
                            Variable v = new Variable(p.VariableName, val);
                            Runner.Instance.AddVariable(v);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Use the ILogger and log the exception
                    }
                }
            }
        }

        [XmlIgnore]
        public virtual ObservableCollection<PropertyDescriptor> PostRunProps
        {
            get
            {
                ObservableCollection<PropertyDescriptor> list = new ObservableCollection<PropertyDescriptor>();
                foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(this))
                {
                    AttributeCollection collection = propDesc.Attributes;
                    foreach (Attribute attribute in collection)
                    {
                        if (attribute.GetType() == typeof(PostRun))
                        {
                            list.Add(propDesc);
                        }
                    }
                }
                return list;
            }
        }

        [XmlIgnore]
        public virtual ObservableCollection<string> PostRunPropsString
        {
            get
            {
                ObservableCollection<string> list = new ObservableCollection<string>();
                foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(this))
                {
                    AttributeCollection collection = propDesc.Attributes;
                    foreach (Attribute attribute in collection)
                    {
                        if (attribute.GetType() == typeof(PostRun))
                        {
                            list.Add(propDesc.Name);
                        }
                    }
                }
                return list;
            }
        }

        #endregion

        #region Property
        /// <summary>
        /// Gets/Sets the Child test cases
        /// </summary>
        [XmlElement(Type = typeof(TestCaseSerializer))]
        public ObservableCollection<TestCase> Children
        {
            get { return children; }
            set { children = value; }
        }

        /// <summary>
        /// Gets or sets the post run values.
        /// </summary>
        /// <value>
        /// The post run values.
        /// </value>
        public ObservableCollection<PostRunPairs> PostValues { get; set; }

        /// <summary>
        /// Gets/Sets if the test case is a parent
        /// </summary>
        [XmlIgnore]
        public virtual bool IsParent { get; private set; }

        /// <summary>
        /// Gets/sets if the test case can provide a result
        /// </summary>
        public virtual bool CanResult { get; set; }

        /// <summary>
        /// Gets/sets the exit on error
        /// </summary>
        public virtual bool ExitOnError { get; set; }

        /// <summary>
        /// If set to yes, the test case is excluded from execution
        /// </summary>
        public virtual bool ExcludeFromExecution { get; set;}

        /// <summary>
        /// The description of the test case
        /// </summary>
        [Description]
        public virtual String Description { get; set; }

        /// <summary>
        /// The name of the test case
        /// </summary>
        [DisplayName]
        public virtual String Name { get; set; }

        /// <summary>
        /// Gets/sets the expected test status
        /// </summary>
        public virtual TestStatus Expected { get; set; }

        /// <summary>
        /// Override and set this property to true if you want the runner to exit the current branch on an error
        /// </summary>
        public virtual bool ExitBranchOnError { get; set; }

        /// <summary>
        /// Override and set the value of this property in milliseconds to inform the test runner to wait before running the next test
        /// </summary>
        public virtual int WaitTime { get; set; }

        /// <summary>
        /// The current test case result
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        internal virtual TestResult CurrentResult
        {
            get
            {
                return result;
            }
        }

        /// <summary>
        /// A bitmap by the size of 16x16 to be shown next to the test case
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        protected static ImageSource Icon
        {
            get 
            { 
                return null; 
            }
        }

        /// <summary>
        /// Returns a user control which will be used as a editor for the test
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public TestEditControl Control
        {
            get
            {
                return new TestEditControl();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to take screen shot.
        /// </summary>
        /// <value>
        ///   <c>true</c> if you want to take screen shot; otherwise, <c>false</c>.
        /// </value>
        public virtual bool TakeScreenShot { get; set; }

        /// <summary>
        /// Gets/Sets if the result is expanded
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets/Sets if the result is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets if the test case is required or not
        /// </summary>
        public bool IsRequired { get; set; }
        #endregion

        #region Members
        protected ObservableCollection<TestCase> children = new ObservableCollection<TestCase>();
        protected TestCase parent;
        protected TestStatus actualResult = TestStatus.None;
        protected TestResult result;
        #endregion

    }
}
