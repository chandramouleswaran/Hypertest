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
using System.Collections.ObjectModel;

using Hypertest.Core.Utils;

namespace Hypertest.Core.GUI
{
    /// <summary>
    /// Interaction logic for TestsPane.xaml
    /// </summary>
    public partial class TestsPane : UserControl
    {
        #region Members
        private Point startingPoint;
        private bool isDragging;
        private TestScenario testScenario;
        private ScenarioManager manager;
        private ObservableCollection<TestListItem> listItems;
        #endregion

        public TestsPane()
        {
            InitializeComponent();
            listItems = FileUtils.LoadTests;
            listBox1.DataContext = listItems;
        }

        #region Drap

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed ||
                    e.RightButton == MouseButtonState.Pressed && !this.isDragging)
                {
                    Point position = e.GetPosition(listBox1);
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
            //Set Mode to "self"
            dataObj.SetData("mode", "other");
            TestListItem selection = this.listBox1.SelectedItem as TestListItem;
            if(selection != null)
            {
                dataObj.SetData("source", selection.NewInstance());
                
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDrop.DoDragDrop(this.listBox1, dataObj, dde);
            }
            this.isDragging = false;
        }

        private void listBox1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startingPoint = e.GetPosition(null);
        }
        #endregion

        #region Commands
        private void CommandBinding_CanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_RefreshExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            listItems = FileUtils.LoadTests;
            listBox1.DataContext = listItems;
        }
        #endregion

    }
}
