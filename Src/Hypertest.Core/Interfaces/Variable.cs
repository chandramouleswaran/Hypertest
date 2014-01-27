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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Hypertest.Core.Interfaces
{
    /// <summary>
    ///     DataType enum
    /// </summary>
    public enum DataType
    {
        Number,
        String,
        Boolean,
        Object
    }

    /// <summary>
    ///     The variable class
    /// </summary>
    [DataContract]
    public class Variable : INotifyPropertyChanged
    {
        #region Members

        private string _name;
        private DataType _type;
        private object _value;

        #endregion

        #region Property

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }


        [DataMember]
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged();
            }
        }

        [DataMember]
        public DataType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Overrides
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var v = obj as Variable;
            if (v != null)
            {
                return Name == v.Name;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}={2}", _type, _name, _value);
        }

        #endregion

        #region CTOR

        public Variable(string name, string strValue)
        {
            double dVal;
            bool bVal;
            Name = name;
            if (double.TryParse(strValue, out dVal))
            {
                Value = dVal;
                Type = DataType.Number;
            }
            else if (bool.TryParse(strValue, out bVal))
            {
                Value = bVal;
                Type = DataType.Boolean;
            }
            else
            {
                Value = strValue;
                Type = DataType.String;
            }
        }

        public Variable(string name, object val)
        {
            Name = name;
            if (val != null)
            {
                Type = DataType.Object;
                Type t = val.GetType();

                if (t == typeof (string) || t == typeof (String))
                {
                    Type = DataType.String;
                }
                if (t == typeof (double) || t == typeof (Double) || t == typeof (int) || t == typeof (Int32))
                {
                    Type = DataType.Number;
                }
                if (t == typeof (bool) || t == typeof (Boolean))
                {
                    Type = DataType.Boolean;
                }
                Value = val;
            }
        }

        public Variable()
        {
            Name = "";
            Type = DataType.String;
            Value = "";
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}