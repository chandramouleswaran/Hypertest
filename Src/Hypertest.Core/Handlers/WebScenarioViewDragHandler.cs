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
            TestCase tc = dropInfo.TargetItem as TestCase;
            if (tc != null)
            {
                TestScenario scenario = tc.Scenario;
                scenario.Manager.EndChangeSetBatch();
            }
        }
    }
}
