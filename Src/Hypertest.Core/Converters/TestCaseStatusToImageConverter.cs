#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;
using Hypertest.Core.Tests;
using Hypertest.Core.Utils;

namespace Hypertest.Core.Converters
{
    internal class TestCaseStatusToImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TestRunState state;
            TestCaseResult result;

            Enum.TryParse(values[0].ToString(), out state);
            Enum.TryParse(values[1].ToString(), out result);

            switch (state)
            {
                case TestRunState.NotStarted:
                    return null;
                case TestRunState.Executing:
                    return ResourceHelper.LoadBitmapFromResource("Images/Executing.png");
                case TestRunState.Done:
                    switch (result)
                    {
                        case TestCaseResult.Failed:
                            return ResourceHelper.LoadBitmapFromResource("Images/Failed.png");
                        case TestCaseResult.Passed:
                            return ResourceHelper.LoadBitmapFromResource("Images/Passed.png");
                        default:
                            return ResourceHelper.LoadBitmapFromResource("Images/None.png");
                    }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}