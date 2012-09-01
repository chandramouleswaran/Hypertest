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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Hypertest.Core.Utils;

namespace Hypertest.Core.GUI
{
    /// <summary>
    /// Interaction logic for TestTree.xaml
    /// </summary>
    public partial class TestTree
    {
        #region Members
        private Point startingPoint;
        private bool isDragging;
        private TestScenario scenario;
        private ScenarioManager manager;
        #endregion

        #region CTOR
        public TestTree()
        {
            InitializeComponent();
        }
        #endregion

        #region Drag Drop
        private void treeView1_Drop(object sender, DragEventArgs e)
        {
            try
            {
                DataObject item = e.Data as DataObject;

                if (item != null)
                {
                    String mode = item.GetData("mode") as String;
                    if (mode != null)
                    {
                        switch (mode)
                        {
                                //THE CASE WHERE WE ARE PLAYING AROUND WITH THE TREE
                            case "self":
                                if (!this.isDragging)
                                {
                                    return;
                                }
                                TestCase source = item.GetData("source") as TestCase;
                                TreeViewItem tviNew = FindVisualParent<TreeViewItem>(e.OriginalSource as UIElement);
                                TreeView t = FindVisualParent<TreeView>(e.OriginalSource as UIElement);

                                TestCase newParent = null;
                                TestCase oldParent = null;

                                TreeViewItem tviOld = item.GetData("parent") as TreeViewItem;

                                if (tviOld != null)
                                {
                                    oldParent = tviOld.Header as TestCase;
                                }

                                if (tviNew != null)
                                {
                                    newParent = tviNew.Header as TestCase;
                                }
                                else if (t != null)
                                {
                                    newParent = this.scenario;
                                }

                                if (oldParent == null)
                                {
                                    oldParent = this.scenario;
                                }

                                if (newParent == source || (oldParent == newParent && oldParent != this.scenario))
                                {
                                    e.Handled = true;
                                    return;
                                }

                                oldParent.Children.Remove(source);
                                TestCase src = source.Clone();
                                src.IsSelected = true;
                                newParent.Children.Add(src);
                                newParent.IsExpanded = true;
                                this.manager.AddScenario(this.scenario);
                                this.Refresh();
                                e.Handled = true;
                                break;
                                //THE CASE WHEN WE DRAG AND DROP FROM ANOTHER PANE
                            case "other":
                                source = item.GetData("source") as TestCase;
                                tviNew = FindVisualParent<TreeViewItem>(e.OriginalSource as UIElement);
                                t = FindVisualParent<TreeView>(e.OriginalSource as UIElement);
                                newParent = null;

                                if (tviNew != null)
                                {
                                    newParent = tviNew.Header as TestCase;
                                }
                                else if (t != null)
                                {
                                    newParent = this.scenario;
                                }

                                if (newParent != null)
                                {
                                    src = source.Clone();
                                    src.IsSelected = true;
                                    newParent.Children.Add(src);
                                    newParent.IsExpanded = true;
                                    this.manager.AddScenario(this.scenario);
                                    this.Refresh();
                                    e.Handled = true;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed ||
                    e.RightButton == MouseButtonState.Pressed && !this.isDragging)
                {
                    Point position = e.GetPosition(treeView1);
                    if (Math.Abs(position.X - startingPoint.X) >
                            SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - startingPoint.Y) >
                            SystemParameters.MinimumVerticalDragDistance)
                    {
                        StartDrag(e);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            this.isDragging = true;

            DataObject dataObj = new DataObject();
            TreeViewItem item = treeView1.Tag as TreeViewItem;
            //Set Mode to "self"
            dataObj.SetData("mode", "self");
            dataObj.SetData("source", this.treeView1.SelectedItem);
            if (item != null)
            {
                TreeViewItem parent = FindVisualParent<TreeViewItem>(item);
                if (parent != null)
                {
                    dataObj.SetData("parent", parent);
                }
            }

            DragDropEffects dde = DragDropEffects.Move;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                dde = DragDropEffects.All;
            }
            DragDrop.DoDragDrop(this.treeView1, dataObj, dde);
            
            this.isDragging = false;
        }

        private void treeView1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startingPoint = e.GetPosition(null);
        }
        #endregion

        #region Events
        private void treeView1_OnItemSelected(object sender, RoutedEventArgs e)
        {
            treeView1.Tag = e.OriginalSource;
        }
        #endregion

        #region Property
        public TreeView TreeView
        {
            get
            {
                return treeView1;
            }
        }

        public TestScenario Current
        {
            get
            {
                return this.scenario;
            }
        }
        #endregion

        #region Static
        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }
        #endregion

        #region Commands
        #region Cut
        private void CommandBinding_CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = treeView1.SelectedItem != null;
        }

        private void CommandBinding_CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Copy();
            this.Delete();
            manager.AddScenario(this.scenario);
            this.Refresh();
        }
        #endregion

        #region Copy
        private void CommandBinding_CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = treeView1.SelectedItem != null;
        }

        private void CommandBinding_CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Copy();
        }
        #endregion

        #region Paste
        private void CommandBinding_CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((treeView1.Items.Count == 0 && treeView1.IsFocused) || treeView1.SelectedItem != null);
        }

        private void CommandBinding_PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestCase copyValue = Clipboard.GetData("hypertest") as TestCase;
            if (copyValue != null)
            {
                this.scenario.Children.Add(copyValue);
                manager.AddScenario(this.scenario);
                this.Refresh();
            }
            treeView1.Focus();
            e.Handled = true;
        }
        #endregion

        #region Delete
        private void CommandBinding_CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = treeView1.SelectedItem != null;
        }

        private void CommandBinding_DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Delete();
            manager.AddScenario(this.scenario);
            this.Refresh();
            e.Handled = true;
        }
        #endregion

        #region Undo
        private void CommandBinding_CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = manager.CanUndo;
        }

        private void CommandBinding_UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            manager.Undo();
            this.Refresh();
            e.Handled = true;
        }
        #endregion

        #region Redo
        private void CommandBinding_CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = manager.CanRedo;
        }

        private void CommandBinding_RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            manager.Redo();
            this.Refresh();
            e.Handled = true;
        }
        #endregion
        #endregion

        #region Methods
        private void Copy()
        {
            TestCase toCopy = treeView1.SelectedItem as TestCase;
            if (toCopy != null)
            {
                TestCase cloned = toCopy.Clone();
                Clipboard.Clear();
                Clipboard.SetData("hypertest", cloned);
            }
        }

        private void Delete()
        {
            TreeViewItem item = treeView1.Tag as TreeViewItem;
            if (item != null)
            {
                TestCase testCase = item.Header as TestCase;
                if (item != null && testCase != null)
                {
                    TreeViewItem parent = FindVisualParent<TreeViewItem>(item);

                    if (parent != null)
                    {
                        TestCase t = parent.Header as TestCase;
                        if (t != null)
                        {
                            t.Children.Remove(testCase);
                        }
                    }
                    else
                    {
                        // The selection is a part of the scenario.
                        this.scenario.Children.Remove(testCase);
                    }
                }
            }
        }

        public void SetScenario(TestScenario testScenario)
        {
            this.scenario = testScenario;
            manager = new ScenarioManager();
            manager.AddScenario(this.scenario);
            this.Refresh();
        }

        private void Refresh()
        {
            this.scenario = manager.Current;
            this.DataContext = this.scenario;
            treeView1.Focus();
        }
        #endregion
    }
}
