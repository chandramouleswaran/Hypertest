#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Windows;
using GongSolutions.Wpf.DragDrop;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Handlers
{
    public class WebScenarioViewDragHandler : DefaultDragHandler
    {
        private TestCase _testCase;

        public override void StartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem != null)
            {
                if (dragInfo.SourceItem.GetType() == typeof (WebTestScenario))
                {
                    dragInfo.Effects = DragDropEffects.None;
                }
                else
                {
                    _testCase = dragInfo.SourceItem as TestCase;
                    if (_testCase != null)
                    {
                        TestScenario scenario = _testCase.Scenario;
                        scenario.Manager.BeginChangeSetBatch("Drag and Drop");
                    }
                    base.StartDrag(dragInfo);
                }
            }
        }

        public override void DragCancelled()
        {
            if (_testCase != null)
            {
                _testCase.Scenario.Manager.EndChangeSetBatch();
            }
            base.DragCancelled();
        }

        public override void Dropped(IDropInfo dropInfo)
        {
            var tc = dropInfo.TargetItem as TestCase;
            if (tc != null)
            {
                TestScenario scenario = tc.Scenario;
                scenario.Manager.EndChangeSetBatch();
            }
        }
    }
}