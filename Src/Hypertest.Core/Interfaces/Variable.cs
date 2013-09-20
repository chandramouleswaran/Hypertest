using System;
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
    public class Variable
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public DataType Type { get; set; }

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

                if (t == typeof(string) || t == typeof(String))
                {
                    this.Type = DataType.String;
                }
                if (t == typeof(double) || t == typeof(Double) || t == typeof(int) || t == typeof(Int32))
                {
                    this.Type = DataType.Number;
                }
                if (t == typeof(bool) || t == typeof(Boolean))
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
    }
}
