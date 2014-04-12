using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Results;
using Hypertest.Core.Tests;

namespace Hypertest.Report.File
{
    public class FileReport
    {
        private IRunner _runner;
        private ITestRegistry _registry;

        public FileReport(IRunner runner, ITestRegistry registry)
        {
            _runner = runner;
            _registry = registry;
        }

        public void Process()
        {
            try
            {
                //Save the result in its location
                using (var writer = new FileStream(_runner.RunFolder + Path.DirectorySeparatorChar + "Result.wtr", FileMode.Create, FileAccess.Write))
                {
                    var types = new List<Type>();
                    types.Add(typeof(WebTestScenario));
                    var ser = new DataContractSerializer(typeof(TestResultModel), _registry.Tests.Union(types));
                    ser.WriteObject(writer, this._runner.Result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
