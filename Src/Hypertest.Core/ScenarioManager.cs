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

using System.Collections.Generic;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// The undo/redo manager for the given scenario
    /// </summary>
    public class ScenarioManager
    {
        #region Members
        private readonly List<TestScenario> sList;
        private int location;
        #endregion

        #region CTOR
        public ScenarioManager()
        {
            location = -1;
            this.sList = new List<TestScenario>();
        }
        #endregion

        #region Property
        public bool CanUndo
        {
            get
            {
                return (this.sList.Count > 0) && (this.location > 0);
            }
        }

        public bool CanRedo
        {
            get
            {
                return (this.sList.Count > 0) && (this.location < this.sList.Count - 1);
            }
        }

        public TestScenario Current
        {
            get
            {
                return sList[location].Clone();
            }
        }
        #endregion

        #region Methods
        public void AddScenario(TestScenario curr)
        {
            if (location <= this.sList.Count - 1)
            {
                this.sList.RemoveRange(location + 1, this.sList.Count - location - 1);
            }
            location++;
            sList.Add(curr.Clone());
        }

        public TestScenario Undo()
        {
            if (CanUndo)
            {
                location--;
                return Current;
            }
            return null;
        }

        public TestScenario Redo()
        {
            if (CanRedo)
            {
                location++;
                return Current;
            }
            return null;
        }
        #endregion
    }
}
