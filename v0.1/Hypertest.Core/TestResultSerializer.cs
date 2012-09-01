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
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

using Hypertest.Core.Utils;

namespace Hypertest.Core
{
    /// <summary>
    /// The class which serializes the test case and writes to an XML file
    /// </summary>
    public class TestResultSerializer : IXmlSerializable
    {

        #region Static
        public static implicit operator TestResultSerializer(TestResult p)
        {
            return p == null ? null : new TestResultSerializer(p);
        }

        public static implicit operator TestResult(TestResultSerializer p)
        {
            return p == null ? null : p.Parameters;
        }
        #endregion Static

        #region Constructors
        public TestResultSerializer() { }

        public TestResultSerializer(TestResult parameters)
        {
            this.parameters = parameters;
        }
        #endregion Constructors

        #region Properties
        public TestResult Parameters
        {
            get { return parameters; }
        } 
        TestResult parameters;
        #endregion Properties

        #region IXmlSerializable Implementation
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            String typeName = reader.GetAttribute("type");
            Type type;
            if (typeName == typeof(TestResult).FullName)
            {
                type = typeof(TestResult);
            }
            else
            {
                Dictionary<String, Type> dict = TypeUtils.LoadTestCaseAssemblies(FileUtils.ScanPath);
                type = dict[typeName];
            }
            reader.ReadStartElement();
            this.parameters = (TestResult)new XmlSerializer(type).Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", parameters.GetType().ToString());
            new XmlSerializer(parameters.GetType()).Serialize(writer, parameters);
        }
        #endregion IXmlSerializable Implementation

    }
}
