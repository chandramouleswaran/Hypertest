using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Hypertest.Core.Interfaces
{

    /// <summary>
    /// DataType enum
    /// </summary>
    public enum DataType
    {
        Number,
        String,
        Boolean,
        Object
    }

    /// <summary>
    /// The variable class
    /// </summary>
    [DataContract]
    public class Variable : INotifyPropertyChanged
    {
        #region Members

        private string _name;
        private object _value;
        private DataType _type;

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
            Variable v = obj as Variable;
            if (v != null)
            {
                return this.Name == v.Name;
            }
            return false;
        }

        public override string ToString()
        {
            return _name + "=" + _value.ToString() + " (" + _type.ToString() + ")";
        }

        #endregion

        #region CTOR

        public Variable(string name, string strValue)
        {
            double dVal;
            bool bVal;
            this.Name = name;
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
                this.Value = strValue;
                Type = DataType.String;
            }
        }

        public Variable(string name, object val)
        {
            this.Name = name;
            if (val != null)
            {
                this.Type = DataType.Object;
                Type t = val.GetType();

                if (t == typeof (string) || t == typeof (String))
                {
                    this.Type = DataType.String;
                }
                if (t == typeof (double) || t == typeof (Double) || t == typeof (int) || t == typeof (Int32))
                {
                    this.Type = DataType.Number;
                }
                if (t == typeof (bool) || t == typeof (Boolean))
                {
                    this.Type = DataType.Boolean;
                }
                this.Value = val;
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
