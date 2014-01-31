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
using System.Windows.Data;
using System.Windows.Forms;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Binding = System.Windows.Data.Binding;
using UserControl = System.Windows.Controls.UserControl;

namespace Hypertest.Core.Editors
{
    /// <summary>
    /// Interaction logic for ScenarioFilePathEditor.xaml
    /// </summary>
    public partial class ScenarioFilePathEditor : UserControl, ITypeEditor
    {
        #region CTOR
        public ScenarioFilePathEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Dependency Property
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ScenarioFilePathEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        } 
        #endregion

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Test Scenario files (*.wts) | *.wts";
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                Value = fileDialog.FileName;
            }
            else
            {
                Value = string.Empty;
            }
        } 
        #endregion

        #region ITypeEditor
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ScenarioFilePathEditor.ValueProperty, binding);
            return this;
        } 
        #endregion
    }
}