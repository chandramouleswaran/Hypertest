#region License

// Copyright (c) 2014 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Hypertest.Core.Tests;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Hypertest.Core.Handlers
{
	public class WebScenarioViewDropHandler : DefaultDropHandler
	{
		public override void DragOver(IDropInfo dropInfo)
		{
			var item = dropInfo.VisualTargetItem as TreeViewItem;
			if (item != null)
			{
				var tc = item.DataContext as TestCase;
				if (tc != null)
				{
					if (tc == dropInfo.Data)
					{
						dropInfo.Effects = DragDropEffects.None;
						return;
					}
				}

				var folderTest = item.DataContext as FolderTestCase;
				if (folderTest != null)
				{
					if (folderTest.AreNewItemsAllowed())
					{
						folderTest.IsExpanded = true;
						base.DragOver(dropInfo);
					}
					return;
				}

				base.DragOver(dropInfo);
			}
			else
			{
				var view = dropInfo.VisualTarget as TreeView;
				if (view != null)
				{
					IDropTarget dropHandler = DragDrop.GetDropHandler(view);
					if (dropHandler == this)
					{
						dropInfo.Effects = DragDropEffects.Move;
					}
				}
			}
		}

		public override void Drop(IDropInfo dropInfo)
		{
			var item = dropInfo.VisualTargetItem as TreeViewItem;
			var view = dropInfo.VisualTarget as TreeView;
			bool directlyOverItem = false;

			if (item != null && view != null)
			{
				var result = view.InputHitTest(dropInfo.DropPosition) as UIElement;
				if (result != null)
				{
					var ancestor = result.GetVisualAncestor<TreeViewItem>();
					directlyOverItem = (ancestor != null) && (ancestor == item);
				}
				var ftc = item.DataContext as FolderTestCase;
				if (ftc != null && directlyOverItem)
				{
					int insertIndex = dropInfo.InsertIndex;
					IList destinationList = GetList(dropInfo.TargetCollection);
					IEnumerable data = ExtractData(dropInfo.Data);

					if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
					{
						IList sourceList = GetList(dropInfo.DragInfo.SourceCollection);

						foreach (object o in data)
						{
							int index = sourceList.IndexOf(o);

							if (index != -1)
							{
								sourceList.RemoveAt(index);

								if (sourceList == destinationList && index < insertIndex)
								{
									--insertIndex;
								}
							}
						}
					}

					foreach (object o in data)
					{
						ftc.Children.Add(o as TestCase);
					}
				}
			}
			if (item == null && view != null)
			{
				//Case when you drop anywhere on the tree and not on an item
				TestScenario scenaio = view.DataContext as TestScenario;
				TestCase tc = dropInfo.Data as TestCase;
				if (scenaio != null && tc != null)
				{
				    if (tc.Scenario == scenaio)
				    {
				        scenaio.Children.Remove(tc);
				    }
                    //Clone the dragged object - you never know if the object is dragged from one window to another
				    tc = tc.Clone() as TestCase;
				    tc.IsSelected = true;
					scenaio.Children.Add(tc);
				}
			}
			else if (!directlyOverItem)
			{
				base.Drop(dropInfo);
			}
		}
	}
}