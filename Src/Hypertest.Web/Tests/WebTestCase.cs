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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Hypertest.Core.Attributes;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Tests;
using Hypertest.Web.Elements;
using OpenQA.Selenium;
using Wide.Interfaces.Services;
using Hypertest.Core.Utils;

namespace Hypertest.Web.Tests
{
	public enum ElementQueryType
	{
		ElementID,
		ElementName,
		LinkText,
		XPath,
		ClassName,
		CssSelector,
		TagName,
		WebElement
	}

	[DataContract]
	[Serializable]
	[DisplayName("Search Element")]
	[Description("Searches for an element in the browser window")]
	[Category("Web")]
	[TestImage("Images/Search.png")]
    [ScenarioTypes(typeof(WebTestScenario))]
	public class WebTestCase : TestCase
	{
		#region Members

		private ElementQueryType _queryType;
		private string _elementDescriptor;
		private string _parentVariable;
		private int _elementNumber;
		private bool _recurse;
		#endregion

		#region CTOR

		public WebTestCase()
		{
			Initialize();
		}

		private void Initialize(bool create = true)
		{
			this.Description = "Search for an element";
			this.MarkedForExecution = true;
			this.ElementNumber = 1;
		}

		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the value used along with query type.
		/// </summary>
		[DataMember]
		[DisplayName("Query Descriptor")]
		[Description("Based on the \"Query By\" value, set this field")]
		[Category("Query")]
		[DynamicReadonly("RunState")]
		public String ElementDescriptor
		{
			get { return _elementDescriptor; }
			set
			{
				string oldValue = _elementDescriptor;
				_elementDescriptor = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _elementDescriptor, "Descriptor change");
			}
		}

		/// <summary>
		/// Gets or sets the variable from which you want to query other elements.
		/// </summary>
		[DataMember]
		[DisplayName("Query Variable")]
		[Description("The variable which has a web element")]
		[Category("Query")]
		[DynamicReadonly("RunState")]
		public String ParentVariable
		{
			get { return _parentVariable; }
			set
			{
				string oldValue = _parentVariable;
				_parentVariable = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _parentVariable, "Parent Variable change");
			}
		}

		/// <summary>
		/// Gets or sets how to query the element.
		/// </summary>
		[DataMember]
		[DisplayName("Query By")]
		[Description("How do you want to search for the web element?")]
		[Category("Query")]
		[DynamicReadonly("RunState")]
		public ElementQueryType QueryType
		{
			get { return _queryType; }
			set
			{
				ElementQueryType oldValue = _queryType;
				_queryType = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _queryType, "Element Query Type change");
			}
		}

		/// <summary>
		/// Gets or sets ElementNumber.
		/// </summary>
		[DataMember]
		[DisplayName("Element Number")]
		[Description("Search may return many elements based on the Query. This field needs to be used to pick the correct element")]
		[Category("Settings")]
		[DefaultValue(1)]
		[DynamicReadonly("RunState")]
		public int ElementNumber
		{
			get { return _elementNumber; }
			set
			{
				int oldValue = _elementNumber;
				_elementNumber = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _elementNumber, "Element Number change");
			}
		}

		/// <summary>
		/// Gets or sets ElementNumber.
		/// </summary>
		[DataMember]
		[DisplayName("Recurse?")]
		[Description("Searches the element in all iframes")]
		[Category("Settings")]
		[DefaultValue(true)]
		[DynamicReadonly("RunState")]
		public bool Recurse
		{
			get { return _recurse; }
			set
			{
				bool oldValue = _recurse;
				_recurse = value;
				if (oldValue != value)
					RaisePropertyChangedWithValues(oldValue, _recurse, "Recurse change");
			}
		}
		#endregion

		#region Element Based Properties
		/// <summary>
		/// Gets the total number of elements returned by the search.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public int TotalElements { get; private set; }

		/// <summary>
		/// Gets or sets the total number of elements returned by the search.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public WebElement Element { get; private set; }

		//TODO: LOG EVERY THING
		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string CurrentURL
		{
			get
			{
				try
				{
					return this.Runner.Driver.Url;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string ClassName
		{
			get
			{
				try
				{
					return this.Element.ClassName;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual bool Displayed
		{
			get
			{
				try
				{
					return this.Element.Displayed;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual bool IsEnabled
		{
			get
			{
				try
				{
					return this.Element.Enabled;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual bool Exists
		{
			get
			{
				try
				{
					return this.Element.Exists;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual bool HasFocus
		{
			get
			{
				try
				{
					return this.Element.HasFocus(this.Runner.Driver);
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string ID
		{
			get
			{
				try
				{
					return this.Element.ID;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual int LocationX
		{
			get
			{
				try
				{
					return this.Element.Location.X;
				}
				catch (Exception)
				{
					return -999;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual int LocationY
		{
			get
			{
				try
				{
					return this.Element.Location.Y;
				}
				catch (Exception)
				{
					return -999;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string ElementName
		{
			get
			{
				try
				{
					return this.Element.Name;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual int Height
		{
			get
			{
				try
				{
					return this.Element.Size.Height;
				}
				catch (Exception)
				{
					return -999;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual int Width
		{
			get
			{
				try
				{
					return this.Element.Size.Width;
				}
				catch (Exception)
				{
					return -999;
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string Style
		{
			get
			{
				try
				{
					return this.Element.Style;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string Text
		{
			get
			{
				try
				{
					return this.Element.Text;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string Title
		{
			get
			{
				try
				{
					return this.Element.Title;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string Value
		{
			get
			{
				try
				{
					return this.Element.Value;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[PostRun]
		public virtual string TagName
		{
			get
			{
				try
				{
					return this.Element.TagName;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}
		#endregion

		#region Deserialize

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			Initialize();
		}

		#endregion

		#region Overrides
		protected override void Body()
		{
			this.ActualResult = TestCaseResult.Passed;
			this.Element = GetWebElement(this.Runner.Driver);
			if (this.Element == null || this.Element.InnerElement == null)
			{
				this.Log("Unable to find the requested element in the Web page to perform the action.", LogCategory.Warn, LogPriority.Medium);
				this.ActualResult = TestCaseResult.Failed;
				return;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the query for your web test case based on ElementDescriptor and QueryType
		/// </summary>
		protected By GetElementBy()
		{
			By by;
			this.ElementDescriptor = StringExtensions.Evaluate(this.ElementDescriptor, this.Runner).ToString();
			try
			{
				switch (QueryType)
				{
					case ElementQueryType.ElementID:
						by = By.Id(ElementDescriptor);
						break;
					case ElementQueryType.ElementName:
						by = By.Name(ElementDescriptor);
						break;
					case ElementQueryType.LinkText:
						by = By.PartialLinkText(ElementDescriptor);
						break;
					case ElementQueryType.XPath:
						by = By.XPath(ElementDescriptor);
						break;
					case ElementQueryType.ClassName:
						by = By.ClassName(ElementDescriptor);
						break;
					case ElementQueryType.CssSelector:
						by = By.CssSelector(ElementDescriptor);
						break;
					case ElementQueryType.TagName:
						by = By.TagName(ElementDescriptor);
						break;
					default:
						by = null;
						break;
				}
			}
			catch (Exception)
			{
				by = null;
			}
			return by;
		}

		/// <summary>
		/// Returns the web element based on the query.
		/// </summary>
		/// <param name="driver">The web driver.</param>
		/// <returns></returns>
		protected WebElement GetWebElement(IWebDriver driver)
		{
			By by = GetElementBy();
			WebElement element = null;
			if (by != null)
			{
				if (ElementNumber <= 0)
				{
					ElementNumber = 1;
				}
				if (string.IsNullOrEmpty(this.ParentVariable))
				{
					WebElementCollection c = new WebElementCollection(driver, by, this.Recurse);
					this.TotalElements = c.Count;
					if(TotalElements > ElementNumber - 1)
						element = c[ElementNumber - 1];
				}
				else
				{
					WebElement w = this.Runner.GetVariable(this.ParentVariable.Replace("%", "")).Value as WebElement;
					WebElementCollection c = new WebElementCollection(w, by);
					this.TotalElements = c.Count;
					element = c[ElementNumber - 1];
				}
			}
			else if (!string.IsNullOrEmpty(this.ElementDescriptor))
			{
				Variable v = this.Runner.GetVariable(ElementDescriptor);
				if (v != null)
				{
					element = v.Value as WebElement;
					if (element == null)
					{
						this.Log("Unable to find a web element using the variable", LogCategory.Error, LogPriority.Medium);
					}
				}
				else
				{
					this.Log("Unable to find the variable", LogCategory.Error, LogPriority.Medium);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.ParentVariable))
				{
					this.Log("There were no search parameters defined for this step", LogCategory.Error, LogPriority.Medium);
				}
				else
				{
					element = this.Runner.GetVariable(this.ParentVariable.Replace("%", "")).Value as WebElement;
				}
			}
			return element;
		}
		#endregion
	}
}