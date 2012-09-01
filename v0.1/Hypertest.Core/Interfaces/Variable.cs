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

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// The variable class
    /// </summary>
    [Serializable]
    public class Variable
    {
        public string Name { get; set; }
        public object Value { get; set; }
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
            if(val != null)
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
