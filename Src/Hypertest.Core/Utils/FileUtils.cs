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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.ObjectModel;

namespace Hypertest.Core.Utils
{


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Gets the application path.
        /// </summary>
        public static string AppPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        /// <summary>
        /// Gets the driver path.
        /// </summary>
        public static string DriverPath
        {
            get
            {
                return AppPath + Path.DirectorySeparatorChar + "Driver";
            }
        }

        /// <summary>
        /// Gets the scan path.
        /// </summary>
        public static string ScanPath
        {
            get
            {
                return AppPath + Path.DirectorySeparatorChar + "Test";
            }
        }

        #region Test Case
        static public void SerializeToXML(TestCase test, String filePath)
        {
            TestScenario scenario = test as TestScenario;
            if (scenario != null)
            { 
                TestCaseSerializer serializer = new TestCaseSerializer(scenario);
                XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                XmlWriter writer = XmlWriter.Create(filePath, settings);
                writer.WriteStartElement("Hypertest");
                serializer.WriteXml(writer);
                writer.WriteEndElement();
                writer.Close();
            }
        }

        static public String SerializeToXML(TestCase test)
        {
            TestScenario scenario = test as TestScenario;
            if (scenario != null)
            {
                TestCaseSerializer serializer = new TestCaseSerializer(scenario);
                XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                using (var sw = new StringWriter()) 
                {
                    using (var writer = XmlWriter.Create(sw))
                    {
                        writer.WriteStartElement("Hypertest");
                        serializer.WriteXml(writer);
                        writer.WriteEndElement();
                        writer.Close();
                    }
                    return sw.ToString();
                }
            }
            return "";
        }

        static public TestScenario LoadFromXML(string filePath)
        {
            TestCaseSerializer serializer = new TestCaseSerializer();
            TextReader inputReader = new StreamReader(filePath);
            XmlReader reader = XmlReader.Create(inputReader);
            reader.Read();
            serializer.ReadXml(reader);
            reader.Close();
            inputReader.Close();
            return (TestScenario)serializer.Parameters;
        }

        static public ObservableCollection<TestListItem> LoadTests
        {
            get
            {
                ObservableCollection<TestListItem> collection = null;
                Dictionary<String, Type> vals = TypeUtils.LoadTestCaseAssemblies(ScanPath);
                foreach (Type val in vals.Values)
                {
                    if (collection == null)
                    {
                        collection = new ObservableCollection<TestListItem>();
                    }
                    if (!val.GetCustomAttributes(false).Any(f => f.GetType() == typeof(TestIgnore)))
                    {
                        TestListItem item = new TestListItem(val);
                        String di = item.DisplayName;
                        collection.Add(item);
                    }
                }
                return collection;
            }
        }
        #endregion

        #region Test Result
        static public void SerializeToXML(TestResult result, String filePath)
        {
            if (result != null)
            {
                TestResultSerializer serializer = new TestResultSerializer(result);
                XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                XmlWriter writer = XmlWriter.Create(filePath, settings);
                writer.WriteStartElement("Hypertest");
                serializer.WriteXml(writer);
                writer.WriteEndElement();
                writer.Close();
            }
        }

        static public TestResult LoadResultFromXML(string filePath)
        {
            TestResultSerializer serializer = new TestResultSerializer();
            TextReader inputReader = new StreamReader(filePath);
            XmlReader reader = XmlReader.Create(inputReader);
            reader.Read();
            serializer.ReadXml(reader);
            reader.Close();
            inputReader.Close();
            return serializer.Parameters;
        }
        #endregion
    }
}
