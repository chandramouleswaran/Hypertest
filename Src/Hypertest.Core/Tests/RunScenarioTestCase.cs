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
using Hypertest.Core.Editors;

namespace Hypertest.Core.Tests
{
    [DataContract]
    [Serializable]
    [DisplayName("Run Scenario")]
    [Description("Specify the scenario to run")]
    [Category("General")]
    [TestImage("Images/RunScenario.png")]
    [ScenarioTypes(typeof(TestScenario))]
    public class RunScenarioTestCase : FolderTestCase
    {
        #region Members

        private string _filePath;

        #endregion

        #region CTOR

        public RunScenarioTestCase()
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
        [DisplayName("File Path")]
        [Description("Enter the file path for the scenario to run")]
        [DynamicReadonly("RunState")]
        [Category("Settings")]
        [Editor(typeof(ScenarioFilePathEditor), typeof(ScenarioFilePathEditor))]
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                string oldValue = _filePath;
                _filePath = value;
                if (oldValue != value)
                    RaisePropertyChangedWithValues(oldValue, _filePath, "File path change");
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
        protected override void Setup()
        {
            TestScenario scenario;
            using (var reader = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read))
            {
                var ser = new DataContractSerializer(typeof (WebTestScenario), this.TestRegistry.Tests);
                scenario = (WebTestScenario) ser.ReadObject(reader);
            }

            if (scenario != null)
            {
                scenario.TestRegistry = this.Scenario.TestRegistry;
                scenario.LoggerService = this.Scenario.LoggerService;
                this.Children = scenario.Children;
            }
        }

        public override bool AreNewItemsAllowed()
        {
            return false;
        }
        #endregion
    }
}