#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hypertest.Core.Manager;
using Hypertest.Core.Utils;
using Wide.Interfaces;

namespace Hypertest.Core.Tests
{
    /// <summary>
    ///     Interaction logic for WebTestScenarioView.xaml
    /// </summary>
    public partial class WebTestScenarioView : UserControl, IContentView
    {
        private bool actionInProgress;
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
            actionInProgress = true;
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
            var toCopy = treeView1.SelectedItem as TestCase;
            if (toCopy != null)
            {
                Clipboard.Clear();
                var clone = (TestCase) toCopy.Clone();
                byte[] data = SerializationHelper.SerializeToBinary(clone,
                    scenario.TestRegistry.Tests.ToArray());
                Clipboard.SetDataObject(data, true);
            }
        }

        #endregion

        #region Paste

        private void CommandBinding_CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            TestCase copyValue = null;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof (byte[])))
            {
                copyValue = SerializationHelper.DeserializeFromBinary<TestCase>((byte[]) data.GetData(typeof (byte[])),
                    scenario.TestRegistry.Tests);
            }
            e.CanExecute = (copyValue != null &&
                            (treeView1.Items.Count == 0 && treeView1.IsFocused || treeView1.SelectedItem != null));
        }

        private void CommandBinding_PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestCase copyValue = null;
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof (byte[])))
            {
                copyValue = SerializationHelper.DeserializeFromBinary<TestCase>((byte[]) data.GetData(typeof (byte[])),
                    scenario.TestRegistry.Tests);
            }

            if (copyValue != null)
            {
                var folder = treeView1.SelectedItem as FolderTestCase;
                if (folder != null)
                {
                    scenario.Manager.BeginChangeSetBatch("Pasting");
                    actionInProgress = true;
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
            actionInProgress = true;
            Delete();
            e.Handled = true;
        }

        private void Delete()
        {
            var tc = treeView1.SelectedItem as TestCase;
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

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            scenario = DataContext as WebTestScenario;
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && e.OldValue != null && e.OldValue != e.NewValue)
            {
                scenario.Manager.AddChange(new CommonPropertyChange(e.OldValue, e.NewValue, "IsSelected"), "Selection");
            }
            treeView1.Focus();
            if (actionInProgress)
            {
                scenario.Manager.EndChangeSetBatch();
                actionInProgress = false;
            }
        }
    }
}