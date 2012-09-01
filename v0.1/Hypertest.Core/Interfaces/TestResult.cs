using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

using System.Windows;

using Hypertest.Core.Utils;
using Hypertest.Core.GUI;

namespace Hypertest.Core
{
    /// <summary>
    /// The base class for all test result implementations
    /// </summary>
    [Serializable]
    public class TestResult
    {
        #region CTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        private TestResult()
        {
            resultFor = null;
            actual = TestStatus.None;
            status = TestStatus.Executing;
            children = new ObservableCollection<TestResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        /// <param name="test">The test case.</param>
        public TestResult(TestCase test):this()
        {
            resultFor = test;
            this.Name = test.Name;
            this.Description = test.Description;
            this.TakeScreenShot = test.TakeScreenShot;
            this.WaitTime = test.WaitTime;
            this.Expected = test.Expected;
        }

        #endregion

        #region Members
        protected TestCase resultFor;
        protected TestStatus status, actual;
        protected ObservableCollection<TestResult> children;
        #endregion

        #region Property
        /// <summary>
        /// Gets the test case.
        /// </summary>
        [XmlIgnore]
        public virtual TestCase TestCase
        {
            get { return resultFor; }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public virtual TestStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Gets the actual result.
        /// </summary>
        public virtual TestStatus Actual
        {
            get { return actual; }
            set { actual = value; }
        }

        /// <summary>
        /// Gets the result UI.
        /// </summary>
        public virtual BaseResultControl ResultUI
        {
            get { return new BaseResultControl(); }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public virtual ObservableCollection<TestResult> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets/Sets if the result is expanded
        /// </summary>
        public virtual bool IsExpanded { get; set; }

        /// <summary>
        /// Gets/Sets if the result is selected
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets/Sets the output message
        /// </summary>
        public String OutputMessage { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual String Description { get; set; }

        /// <summary>
        /// The location where the screenshot is stored
        /// </summary>
        public virtual String ScreenShotLocation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual String Name { get; set; }

        /// <summary>
        /// Gets or sets the wait time.
        /// </summary>
        public virtual int WaitTime { get; set; }

        /// <summary>
        /// Gets or sets the expected value.
        /// </summary>
        public virtual TestStatus Expected { get; set; }

        /// <summary>
        /// Gets or sets the take screenshot value.
        /// </summary>
        public virtual bool TakeScreenShot { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Adds a result to the list of children.
        /// </summary>
        /// <param name="result">The child result.</param>
        internal void AddChild(TestResult result)
        {
            if (result != null)
            {
                // Update collection in a thread safe manner
                Application.Current.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal, (Action)(() => this.children.Add(result)));
            }
            else
            {
                Console.WriteLine("Result is NULL for " + resultFor.Description);
            }
        }
        #endregion

    }
}
