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
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Hypertest.Core.Utils
{
    /// <summary>
    /// Status enum to Bitmapsource converted
    /// </summary>
    [ValueConversion(typeof(TestStatus), typeof(BitmapSource))]
    public class StatusToBitmapSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TestStatus)value)
            {
                case TestStatus.Executing:
                    return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Executing);
                case TestStatus.Failed:
                    return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Failed);
                case TestStatus.Passed:
                    return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.Passed);
                default:
                    return Imaging.CreateBitmapSourceFromBitmap(Properties.Resources.None);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
