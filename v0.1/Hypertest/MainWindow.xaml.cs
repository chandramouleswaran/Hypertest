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
using System.Windows.Data;
using System.Windows.Input;

using AvalonDock;
using Hypertest.Core;
using Hypertest.Core.GUI;
using Hypertest.Core.Utils;

namespace Hypertest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Commands
        private void CommandBinding_CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".tsc", Filter = "Test Scenario (.tsc)|*.tsc" };
            if (dlg.ShowDialog() == true)
            {
                CreateScenarioDocument(dlg.FileName);
            }
        }

        private void CommandBinding_CanNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CreateScenarioDocument();
        }

        private void CommandBinding_CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dockManager != null && dockManager.ActiveDocument != null)
            {
                TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
                if (obj != null)
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CommandBinding_SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
            if (obj != null)
            {
                obj.Save();
            }
        }
        
        private void CommandBinding_CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dockManager != null && dockManager.ActiveDocument != null)
            {
                TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
                if (obj != null)
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CommandBinding_SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
            if (obj != null)
            {
                obj.Save(true);
            }
        }

        private void CommandBinding_CanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            while(dockManager.Documents.Count != 0)
            {
                DocumentContent cnt = dockManager.Documents[0];
                TestWindowObject obj = cnt.DataContext as TestWindowObject;
                if (obj != null && Runner.Instance.IsRunning == false)
                {
                    obj.Save();
                    cnt.Close();
                }
            }
            this.Close();
        }

        private void CommandBinding_CanRun(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dockManager != null && dockManager.ActiveDocument != null)
            {
                TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
                if (obj != null && Runner.Instance.IsRunning == false)
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CommandBinding_RunExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
            if (obj != null)
            {
                CreateRunnerDocument(obj.Content.CurrentScenario);
            }
        }

        private void CommandBinding_CanScenarioSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dockManager != null && dockManager.ActiveDocument != null)
            {
                TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
                if (obj != null)
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CommandBinding_ScenarioSettingsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TestWindowObject obj = dockManager.ActiveDocument.DataContext as TestWindowObject;
            if (obj != null)
            {
                ScenarioSettings settings = new ScenarioSettings();
                settings.InitializeForScenario(obj.Content.CurrentScenario);
                bool? val = settings.ShowDialog();
                if (val == true)
                {
                    obj.Content.SetScenario(settings.NewScenario);
                }
            }
        }

        private void CommandBinding_CanStop(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Runner.Instance.IsRunning;
        }

        private void CommandBinding_StopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Runner.Instance.Stop();
        }

        private void CommandBinding_CanShowTestPane(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_ShowTestPaneExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (testPane1.IsVisible)
            {
                testsContent.Close();
            }
            else
            {
                testsContent.Show(dockManager);
            }            
        }
        #endregion

        #region Methods
        private void CreateScenarioDocument(string filePath = "")
        {
            DocumentContent documentContent = new DocumentContent();
            TestWindowObject obj = null;
            if (!String.IsNullOrEmpty(filePath))
            {
                obj = new TestWindowObject(FileUtils.LoadFromXML(filePath), filePath);
            }
            else
            {
                obj = new TestWindowObject(new TestScenario());
                ScenarioSettings settings = new ScenarioSettings();
                settings.InitializeForScenario(obj.Content.CurrentScenario);
                bool? result = settings.ShowDialog();
                if (result == null || result == false)
                {
                    documentContent.Close();
                    return;
                }
                else
                {
                    obj.Content.SetScenario(settings.NewScenario);
                }
            }
            Binding bind1 = new Binding("Title") { Source = obj };
            documentContent.SetBinding(DocumentContent.TitleProperty, bind1);
            Binding bind2 = new Binding("Content") { Source = obj };
            documentContent.SetBinding(DocumentContent.ContentProperty, bind2);
            Binding bind3 = new Binding { Source = obj };
            documentContent.SetBinding(DocumentContent.DataContextProperty, bind3);
            documentContent.Show(dockManager);
            dockManager.ActiveContent = documentContent;
        }

        private void CreateRunnerDocument(TestScenario scenario)
        {
            DocumentContent documentContent = new DocumentContent { Title = "--- Runner ---" };
            ResultWindow window = new ResultWindow();
            documentContent.Content = window;
            window.SetScenario(scenario);
            documentContent.Show(dockManager);
        }
        #endregion
    }
}
