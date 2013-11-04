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
    public partial class WebTestResultView : UserControl, IContentView
    {
        private WebTestScenario scenario;

        public WebTestResultView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            scenario = this.DataContext as WebTestScenario;
        }
    }
}
