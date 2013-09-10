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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace Hypertest.Core.Utils
{
    /// <summary>
    /// Type utils static class for extensions
    /// </summary>
    public static class TypeUtils
    {
        #region Static
        public static Dictionary<String, Type> LoadTestCaseAssemblies(String path)
        {
            Dictionary<String, Type> dict = null;
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                if (dict == null)
                {
                    dict = new Dictionary<String, Type>();
                }

                try
                {
                    Assembly asm = Assembly.LoadFile(dll);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in asm.GetTypes().Where(f => f.IsSubclassOf(typeof(TestCase))))
                    {
                        dict.Add(t.FullName, t);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
            }
            return dict;
        }
        #endregion

        #region Extensions
        public static string DisplayName(this Type value)
        {
            string displayName = value.Name;
            IEnumerable<Object> names = value.GetCustomAttributes(false).Where(f => f.GetType() == typeof(DisplayNameAttribute));
            if(names.Any())
            {
                displayName = (names.ElementAt(0) as DisplayNameAttribute).DisplayName;
            }
            return displayName;
        }
        #endregion
    }
}
