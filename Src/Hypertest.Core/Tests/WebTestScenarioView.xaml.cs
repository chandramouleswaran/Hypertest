using System;
using Hypertest.Core.Manager;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Hypertest.Core.Utils;
using Wide.Interfaces;

namespace Hypertest.Core.Tests
{
    /// <summary>
    /// Interaction logic for WebTestScenarioView.xaml
    /// </summary>
    public partial class WebTestScenarioView : UserControl, IContentView
    {
        private WebTestScenario scenario;

        public WebTestScenarioView()
        {
            InitializeComponent();
        }

        #region Commands
        #region Cut
        private void CommandBinding_CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionIsTest();
        }

        private void CommandBinding_CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scenario.Manager.BeginChangeSetBatch("Cut action");
            Copy();
            Delete();
            e.Handled = true;
        }
        #endregion

        #region Copy
        private void CommandBinding_CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionIsTest();
        }

        private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
            e.Handled = true;
        }

        private void Copy()
        {
            TestCase toCopy = treeView1.SelectedItem as TestCase;
            if (toCopy != null)
            {
                Clipboard.Clear();
                TestCase clone = (TestCase)toCopy.Clone();
                byte[] data = SerializationHelper.SerializeToBinary<TestCase>(clone, this.scenario.TestRegistry.Tests.ToArray());
                Clipboard.SetDataObject(data, true);
            }
        }
        #endregion

        #region Paste
        private void CommandBinding_CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            TestCase copyValue = null;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof(byte[])))
            {
                copyValue = SerializationHelper.DeserializeFromBinary<TestCase>((byte[])data.GetData(typeof(byte[])), this.scenario.TestRegistry.Tests);
            }
            e.CanExecute = (copyValue != null && (treeView1.Items.Count == 0 && treeView1.IsFocused || treeView1.SelectedItem != null));
        }

        private void CommandBinding_PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestCase copyValue = null;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof(byte[])))
            {
                copyValue = SerializationHelper.DeserializeFromBinary<TestCase>((byte[])data.GetData(typeof(byte[])), this.scenario.TestRegistry.Tests);
            }

            if (copyValue != null)
            {
                FolderTestCase folder = treeView1.SelectedItem as FolderTestCase;
                if(folder != null)
                {
                    scenario.Manager.BeginChangeSetBatch("Pasting");
                    folder.IsExpanded = true;
                    folder.Children.Add(copyValue);
                    copyValue.IsSelected = true;
                }
            }
            e.Handled = true;
        }
        #endregion

        #region Delete
        private void CommandBinding_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionIsTest();
        }

        private void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scenario.Manager.BeginChangeSetBatch("Delete action");
            Delete();
            e.Handled = true;
        }

        private void Delete()
        {
            TestCase tc = treeView1.SelectedItem as TestCase;
            if (tc != null)
            {
                tc.Parent.Children.Remove(tc);
            }
        }
        #endregion

        #region Undo
        private void CommandBinding_CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = scenario.Manager.CanUndo();
        }

        private void CommandBinding_UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scenario.Manager.Undo();
            e.Handled = true;
        }
        #endregion

        #region Redo
        private void CommandBinding_CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = scenario.Manager.CanRedo();
        }

        private void CommandBinding_RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            scenario.Manager.Redo();
            e.Handled = true;
        }
        #endregion

        private bool SelectionIsTest()
        {
            if (treeView1.SelectedItem != null)
            {
                if (treeView1.SelectedItem is WebTestScenario)
                {
                    return false;
                }
                if (treeView1.SelectedItem is TestCase)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            scenario = this.DataContext as WebTestScenario;
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && e.OldValue != null && e.OldValue != e.NewValue)
            {
                scenario.Manager.AddChange(new CommonPropertyChange(e.OldValue, e.NewValue, "IsSelected"), "Selection");
            }
            treeView1.Focus();
            if(scenario.Manager.IsBatch)
            {
                scenario.Manager.EndChangeSetBatch();
            }
        }
    }
}
