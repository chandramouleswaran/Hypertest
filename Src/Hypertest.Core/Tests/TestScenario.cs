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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Hypertest.Core.Interfaces;
using Hypertest.Core.Manager;
using Wide.Interfaces.Services;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Hypertest.Core.Tests
{
	/// <summary>
	///     The basic unit of a test scenario in the Hypertest framework
	/// </summary>
	[DataContract]
	[Serializable]
	public abstract class TestScenario : FolderTestCase
	{
		#region Member

		protected StateManager _manager;

		private ObservableCollection<Variable> _variables;
			//TODO: Should this participate in Undo/Redo? Not necessary as it is not viewable and not apparent to end users

		#endregion

		#region CTOR and other initializers

		protected TestScenario()
		{
			Initialize();
			_children.CollectionChanged += _children_CollectionChanged;
			BulkMonitor(this);
			IsSelected = true;
			IsExpanded = true;
			_manager.Clear();
		}

		private void Initialize()
		{
			_manager = new StateManager();
			_manager.StateChange += _manager_StateChange;
			if (_variables == null)
			{
				_variables = new ObservableCollection<Variable>();
			}
		}

		private void _manager_StateChange(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		#endregion

		#region Deserialize

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Initialize();
			foreach (TestCase test in _children)
			{
				test.Parent = this;
			}
			_children.CollectionChanged += _children_CollectionChanged;
			BulkMonitor(this);
			IsSelected = true;
			IsExpanded = true;
			_manager.Clear();
		}

		#endregion

		#region Monitoring functions

		protected void BulkMonitor(TestCase testCase)
		{
			_manager.MonitorObject(testCase);
			var ftc = testCase as FolderTestCase;
			if (ftc != null)
			{
				ftc.Children.CollectionChanged += _children_CollectionChanged;
				_manager.MonitorCollection(ftc.Children);
				foreach (TestCase tc in ftc.Children)
				{
					BulkMonitor(tc);
				}
			}
		}

		protected void BulkUnmonitor(TestCase testCase)
		{
			_manager.UnmonitorObject(testCase);
			var ftc = testCase as FolderTestCase;
			if (ftc != null)
			{
				ftc.Children.CollectionChanged += _children_CollectionChanged;
				_manager.UnmonitorCollection(ftc.Children);
				foreach (TestCase tc in ftc.Children)
				{
					BulkUnmonitor(tc);
				}
			}
		}

		private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (TestCase test in e.NewItems)
				{
					test.Parent = this;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (_manager != null && e.NewItems != null)
				{
					foreach (TestCase test in e.NewItems)
					{
						BulkMonitor(test);
					}
				}
			}

			if (_manager != null && e.OldItems != null)
			{
				foreach (TestCase test in e.OldItems)
				{
					BulkUnmonitor(test);
				}
			}
		}

		#endregion

		#region Methods

		internal void SetDirty(bool p)
		{
			IsDirty = p;
		}

		internal void SetLocation(object info)
		{
			Location = info;
			IsDirty = false;
			RaisePropertyChanged("Location");
		}

		#endregion

		#region Property

		protected internal override ITestRegistry TestRegistry { get; set; }

		protected internal StateManager Manager
		{
			get { return _manager; }
		}

		protected internal override ILoggerService LoggerService { get; set; }

		protected internal override IRunner Runner { get; set; }

		[DataMember]
		[NewItemTypes(typeof (Variable))]
		public virtual ObservableCollection<Variable> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				RaisePropertyChanged();
			}
		}

		#endregion
	}
}