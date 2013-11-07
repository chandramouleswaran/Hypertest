using System;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using Hypertest.Core.Tests;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Hypertest.Core.Handlers
{
    public class WebScenarioViewDropHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            TreeViewItem item = dropInfo.VisualTargetItem as TreeViewItem;
            if (item != null)
            {
                TestCase tc = item.DataContext as TestCase;
                if (tc != null)
                {
                    if (tc == dropInfo.Data)
                    {
                        dropInfo.Effects = DragDropEffects.None;
                        return;
                    }
                }

                FolderTestCase ftc = item.DataContext as FolderTestCase;
                if (ftc != null)
                {
                    ftc.IsExpanded = true;
                    base.DragOver(dropInfo);
                }
            }
            else
            {
                TreeView view = dropInfo.VisualTarget as TreeView;
                if (view != null)
                {
                    IDropTarget target = DragDrop.GetDropHandler(view);
                    if (target == this)
                    {
                        dropInfo.Effects = DragDropEffects.Move;
                    }
                }
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {
            TreeViewItem item = dropInfo.VisualTargetItem as TreeViewItem;
            if (item != null)
            {
                FolderTestCase ftc = item.DataContext as FolderTestCase;
                if (ftc != null)
                {
                    var insertIndex = dropInfo.InsertIndex;
                    var destinationList = GetList(dropInfo.TargetCollection);
                    var data = ExtractData(dropInfo.Data);

                    if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
                    {
                        var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

                        foreach (var o in data)
                        {
                            var index = sourceList.IndexOf(o);

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

                    foreach (var o in data)
                    {
                        ftc.Children.Add(o as TestCase);
                    }
                }
            }
            else
            {
                //base.Drop(dropInfo);
            }
        }
    }
}
