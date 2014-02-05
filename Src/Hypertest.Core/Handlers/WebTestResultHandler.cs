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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Results;
using Hypertest.Core.Tests;
using Microsoft.Practices.Unity;
using Wide.Core.Attributes;
using Wide.Interfaces;
using Wide.Interfaces.Services;

namespace Hypertest.Core.Handlers
{
    [FileContent("Web test Result", "*.wtr", 1)]
    internal class WebTestResultHandler : IContentHandler
    {
        /// <summary>
        ///     The injected container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        ///     The injected logger service
        /// </summary>
        private readonly ILoggerService _loggerService;

        /// <summary>
        ///     The test registry
        /// </summary>
        private readonly ITestRegistry _testRegistry;

        /// <summary>
        ///     Constructor of MDHandler - all parameters are injected
        /// </summary>
        /// <param name="container">The injected container of the application</param>
        /// <param name="loggerService">The injected logger service of the application</param>
        public WebTestResultHandler(IUnityContainer container, ILoggerService loggerService, ITestRegistry testRegistry)
        {
            _container = container;
            _loggerService = loggerService;
            _testRegistry = testRegistry;
        }

        #region IContentHandler Members

        public ContentViewModel NewContent(object parameter)
        {
            return null;
        }

        /// <summary>
        ///     Validates the content by checking if a file exists for the specified location
        /// </summary>
        /// <param name="info">The string containing the file location</param>
        /// <returns>True, if the file exists and has a .wtr extension - false otherwise</returns>
        public bool ValidateContentType(object info)
        {
            var location = info as string;
            string extension = "";

            if (location == null)
            {
                return false;
            }

            extension = Path.GetExtension(location);
            return File.Exists(location) && extension == ".wtr";
        }

        /// <summary>
        ///     Opens a file and returns the corresponding WebTestScenarioViewModel
        /// </summary>
        /// <param name="info">The string location of the file</param>
        /// <returns>The <see cref="WebTestScenarioViewModel" /> for the file.</returns>
        public ContentViewModel OpenContent(object info)
        {
            var location = info as string;
            if (location != null)
            {
                try
                {
                    TestResultModel model;
                    using (var reader = new FileStream(location, FileMode.Open, FileAccess.Read))
                    {
                        var types = new List<Type>();
                        types.Add(typeof(WebTestScenario));
                        var ser = new DataContractSerializer(typeof(TestResultModel), _testRegistry.Tests.Union(types));
                        model = (TestResultModel)ser.ReadObject(reader);
                    }

                    var vm = _container.Resolve<WebTestResultViewModel>();
                    var view = _container.Resolve<WebTestResultView>();

                    //Model details
                    model.SetLocation(info);
                    model.Scenario.TestRegistry = _testRegistry;
                    model.Scenario.LoggerService = _loggerService;

                    //Set the model and view
                    vm.SetModel(model);
                    vm.SetView(view);
                    vm.Title = Path.GetFileName(location);
                    vm.View.DataContext = model;
                    view.Focus();
                    model.SetDirty(false);
                    model.Scenario.PauseStateManager();

                    return vm;
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }

        public ContentViewModel OpenContentFromId(string contentId)
        {
            string[] split = Regex.Split(contentId, ":##:");
            if (split.Count() == 2)
            {
                string identifier = split[0];
                string path = split[1];
                if (identifier == "FILE" && File.Exists(path))
                {
                    return OpenContent(path);
                }
            }
            return null;
        }

        /// <summary>
        ///     Saves the content of the WebTestResultHandler
        /// </summary>
        /// <param name="contentViewModel">This needs to be a WebTestResultHandler that needs to be saved</param>
        /// <param name="saveAs">Pass in true if you need to Save As?</param>
        /// <returns>true, if successful - false, otherwise</returns>
        public virtual bool SaveContent(ContentViewModel contentViewModel, bool saveAs = false)
        {
            return false;
        }

        /// <summary>
        ///     Validates the content from an ID - the ContentID from the ContentViewModel
        /// </summary>
        /// <param name="contentId">The content ID which needs to be validated</param>
        /// <returns>True, if valid from content ID - false, otherwise</returns>
        public bool ValidateContentFromId(string contentId)
        {
            string[] split = Regex.Split(contentId, ":##:");
            if (split.Count() == 2)
            {
                string identifier = split[0];
                string path = split[1];
                if (identifier == "FILE" && ValidateContentType(path))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}