using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hypertest.Core.Tests;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using Wide.Core.Attributes;
using Wide.Interfaces;
using Wide.Interfaces.Services;
using Hypertest.Core.Interfaces;

namespace Hypertest.Core.Handlers
{
    [FileContent("Web test scenario", "*.wts", 1)]
    [NewContent("Web test scenario files", 1, "Creates a Web test scenario", "pack://application:,,,/Hypertest;component/Hypertest.png")]
    internal class WebTestScenarioHandler : IContentHandler
    {
        /// <summary>
        /// The injected container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// The injected logger service
        /// </summary>
        private readonly ILoggerService _loggerService;

        /// <summary>
        /// The test registry
        /// </summary>
        private readonly ITestRegistry _testRegistry;

        /// <summary>
        /// The save file dialog
        /// </summary>
        private SaveFileDialog _dialog;

        /// <summary>
        /// Constructor of MDHandler - all parameters are injected
        /// </summary>
        /// <param name="container">The injected container of the application</param>
        /// <param name="loggerService">The injected logger service of the application</param>
        public WebTestScenarioHandler(IUnityContainer container, ILoggerService loggerService, ITestRegistry testRegistry)
        {
            _container = container;
            _loggerService = loggerService;
            _testRegistry = testRegistry;
            _dialog = new SaveFileDialog();
        }

        #region IContentHandler Members

        public ContentViewModel NewContent(object parameter)
        {
            var vm = _container.Resolve<WebTestScenarioViewModel>();
            var model = _container.Resolve<WebTestScenario>();
            model.Description = "wow another scenario!";
            model.Children.Add(new FolderTestCase() { Description = "My dear folder" });
            model.IsSelected = true;
            model.Manager.Clear();
            var view = _container.Resolve<WebTestScenarioView>();

            //Model details
            _loggerService.Log("Creating a new simple file using MDHandler", LogCategory.Info, LogPriority.Low);

            //Set the model and view
            vm.SetModel(model);
            vm.SetView(view);
            vm.Title = "untitled-WTS";
            vm.View.DataContext = model;
            vm.SetHandler(this);
            model.SetDirty(true);
            model.TestRegistry = _testRegistry;
            model.LoggerService = _loggerService;

            return vm;
        }

        /// <summary>
        /// Validates the content by checking if a file exists for the specified location
        /// </summary>
        /// <param name="info">The string containing the file location</param>
        /// <returns>True, if the file exists and has a .md extension - false otherwise</returns>
        public bool ValidateContentType(object info)
        {
            string location = info as string;
            string extension = "";

            if (location == null)
            {
                return false;
            }

            extension = Path.GetExtension(location);
            return File.Exists(location) && extension == ".wts";
        }

        /// <summary>
        /// Opens a file and returns the corresponding MDViewModel
        /// </summary>
        /// <param name="info">The string location of the file</param>
        /// <returns>The <see cref="MDViewModel"/> for the file.</returns>
        public ContentViewModel OpenContent(object info)
        {
            var location = info as string;
            if (location != null)
            {
                try
                {
                    WebTestScenario model;
                    using (FileStream reader = new FileStream(location, FileMode.Open, FileAccess.Read))
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(WebTestScenario), _testRegistry.Tests);
                        model = (WebTestScenario)ser.ReadObject(reader);
                    }

                    WebTestScenarioViewModel vm = _container.Resolve<WebTestScenarioViewModel>();
                    var view = _container.Resolve<WebTestScenarioView>();

                    //Model details
                    model.SetLocation(info);
                    model.SetDirty(false);
                    model.TestRegistry = _testRegistry;
                    model.LoggerService = _loggerService;

                    //Set the model and view
                    vm.SetModel(model);
                    vm.SetView(view);
                    vm.Title = Path.GetFileName(location);
                    vm.View.DataContext = model;
                    view.Focus();

                    return vm;
                }
                catch (Exception e)
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
        /// Saves the content of the TextViewModel
        /// </summary>
        /// <param name="contentViewModel">This needs to be a TextViewModel that needs to be saved</param>
        /// <param name="saveAs">Pass in true if you need to Save As?</param>
        /// <returns>true, if successful - false, otherwise</returns>
        public virtual bool SaveContent(ContentViewModel contentViewModel, bool saveAs = false)
        {
            var scenarioViewModel = contentViewModel as WebTestScenarioViewModel;

            if (scenarioViewModel == null)
            {
                _loggerService.Log("ContentViewModel needs to be a Web test scenario to save details", LogCategory.Exception, LogPriority.High);
                throw new ArgumentException("ContentViewModel needs to be a WebTestScenarioViewModel to save details");
            }

            var scenario = scenarioViewModel.Model as WebTestScenario;

            if (scenario == null)
            {
                _loggerService.Log("WebTestScenarioViewModel does not have a WebTestScenario which should have the content", LogCategory.Exception, LogPriority.High);
                throw new ArgumentException("WebTestScenarioViewModel does not have a WebTestScenario which should have the text");
            }

            var location = scenario.Location as string;

            if (location == null)
            {
                //If there is no location, just prompt for Save As..
                saveAs = true;
            }

            if (saveAs)
            {
                if (location != null)
                    _dialog.InitialDirectory = Path.GetDirectoryName(location);

                _dialog.CheckPathExists = true;
                _dialog.DefaultExt = "wts";
                _dialog.Filter = "Web test scenario files (*.wts)|*.wts";

                if (_dialog.ShowDialog() == true)
                {
                    location = _dialog.FileName;
                    scenario.SetLocation(location);
                    scenarioViewModel.Title = Path.GetFileName(location);
                    try
                    {
                        using (FileStream writer = new FileStream(location, FileMode.Create, FileAccess.Write))
                        {
                            DataContractSerializer ser = new DataContractSerializer(typeof(WebTestScenario), _testRegistry.Tests);
                            ser.WriteObject(writer, scenario);
                            scenario.SetDirty(false);
                        }
                        return true;
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Log(exception.Message, LogCategory.Exception, LogPriority.High);
                        _loggerService.Log(exception.StackTrace, LogCategory.Exception, LogPriority.High);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    using (FileStream writer = new FileStream(location, FileMode.Create, FileAccess.Write))
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(WebTestScenario), _testRegistry.Tests);
                        ser.WriteObject(writer, scenario);
                        scenario.SetDirty(false);
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    _loggerService.Log(exception.Message, LogCategory.Exception, LogPriority.High);
                    _loggerService.Log(exception.StackTrace, LogCategory.Exception, LogPriority.High);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the content from an ID - the ContentID from the ContentViewModel
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
