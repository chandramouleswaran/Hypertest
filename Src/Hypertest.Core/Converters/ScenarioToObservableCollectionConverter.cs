using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Hypertest.Core.Tests;

namespace Hypertest.Core.Converters
{
    class ScenarioToObservableCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestScenario scenario = value as TestScenario;
            if(scenario != null)
            {
                ObservableCollection<TestScenario> list = new ObservableCollection<TestScenario>();
                list.Add(scenario);
                return list;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<TestScenario> list = value as ObservableCollection<TestScenario>;
            if(list != null)
            {
                return list[0];
            }
            return null;
        }
    }
}
