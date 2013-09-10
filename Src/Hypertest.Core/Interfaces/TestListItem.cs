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
using System.Linq;
using System.Windows.Media;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestListItem
    {
        private readonly Type type;
        private readonly string displayName;

        public TestListItem(Type val)
        {
            this.type = val;
            this.displayName = val.DisplayName();
        }

        public String DisplayName 
        { 
            get
            {
                return this.displayName;
            }
        }

        public ImageSource Source
        {
            get
            {
                return this.type.GetProperties().Where(f => f.Name == "Icon").Select(p => p.GetValue(null, null)).Select(v => v as ImageSource).FirstOrDefault();
            }
        }

        public object NewInstance()
        {
            return Activator.CreateInstance(type);
        }
    }
}
